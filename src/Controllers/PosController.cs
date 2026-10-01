using System.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
using QuanLiQuanCaffee.ViewModels;
namespace QuanLiQuanCaffee.Controllers;

[Authorize(Roles = "Admin,Staff")]
[AutoValidateAntiforgeryToken]
public class PosController : Controller
{
    private readonly AppDbContext db;
    public PosController(AppDbContext db) { this.db = db; }
    public async Task<IActionResult> Tables() => View(await db.Tables.AsNoTracking().OrderBy(t => t.Name).ToListAsync());
    public async Task<IActionResult> Index(int tableId)
    {
        var table = await db.Tables.AsNoTracking().FirstOrDefaultAsync(t => t.TableID == tableId);
        if (table == null) return NotFound();
        return View(new PosVm { Table = table,
            Products = await db.Products.AsNoTracking().Include(p => p.Category).OrderBy(p => p.CategoryID).ThenBy(p => p.Name).ToListAsync(),
            Categories = await db.Categories.AsNoTracking().OrderBy(c => c.CategoryID).ToListAsync() });
    }
    private Task<Order?> Open(int tableId) => db.Orders.Include(o => o.Details).ThenInclude(d => d.Product)
        .FirstOrDefaultAsync(o => o.TableID == tableId && o.Status == "Đang xử lý" && !o.IsPaid);
    private static decimal Total(Order o) => o.Details.Sum(d => (decimal)d.Price * d.Quantity);
    public async Task<IActionResult> Cart(int tableId)
    {
        var order = await Open(tableId);
        return Json(new { orderId = order?.OrderID ?? 0, total = order == null ? 0m : Total(order),
            items = order?.Details.Select(d => new { id = d.DetailID, name = d.Product?.Name ?? "Món đã lưu",
                quantity = d.Quantity, price = d.Price, d.Size, d.Sugar, d.Ice, d.Topping, d.Note }).ToArray() });
    }
    public class AddInput
    {
        [Range(1,int.MaxValue)] public int TableId { get; set; }
        [Range(1,int.MaxValue)] public int ProductId { get; set; }
        [Range(1,99)] public int Quantity { get; set; } = 1;
        [Required] public string Size { get; set; } = "M";
        public int Sugar { get; set; } = 100;
        public int Ice { get; set; } = 100;
        [Required] public string Topping { get; set; } = "Không";
        [MaxLength(200)] public string? Note { get; set; }
    }
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddInput input)
    {
        if (!ModelState.IsValid) return BadRequest(new { message = "Thông tin món không hợp lệ." });
        using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var table = await db.Tables.FindAsync(input.TableId);
        var product = await db.Products.FindAsync(input.ProductId);
        if (table == null || product == null || !product.IsAvailable)
            return BadRequest(new { message = "Bàn hoặc món không tồn tại, hoặc món đã hết." });
        if (!double.IsFinite(product.Price) || product.Price < 0 || product.Price > 10000000)
            return BadRequest(new { message = "Giá món chưa hợp lệ." });
        if (!product.SupportsCustomization) { input.Size="M"; input.Sugar=100; input.Ice=100; input.Topping="Không"; }
        var toppings = new Dictionary<string,decimal> { ["Không"]=0, ["Trân châu"]=5000, ["Thạch trái cây"]=5000, ["Kem cheese"]=10000 };
        if (!(new[]{"M","L"}).Contains(input.Size) || !(new[]{0,30,50,70,100}).Contains(input.Sugar)
            || !(new[]{0,50,100}).Contains(input.Ice) || !toppings.ContainsKey(input.Topping))
            return BadRequest(new { message = "Tùy chọn món không hợp lệ." });
        var unit = (decimal)product.Price + (input.Size=="L" ? 7000m : 0m) + toppings[input.Topping];
        var order = await Open(input.TableId);
        if (order == null) { order = new Order { TableID=input.TableId, Status="Đang xử lý", OrderTime=DateTime.Now }; db.Orders.Add(order); }
        var note = input.Note?.Trim() ?? "";
        var line = order.Details.FirstOrDefault(d => d.ProductID==input.ProductId && d.Size==input.Size && d.Sugar==input.Sugar
            && d.Ice==input.Ice && d.Topping==input.Topping && d.Note==note && (decimal)d.Price==unit);
        if (line != null && line.Quantity + input.Quantity > 99) return BadRequest(new { message="Tối đa 99 phần cho một dòng món." });
        if (line == null) order.Details.Add(new OrderDetail { ProductID=input.ProductId, Quantity=input.Quantity,
            Price=(double)unit, Size=input.Size, Sugar=input.Sugar, Ice=input.Ice, Topping=input.Topping, Note=note });
        else line.Quantity += input.Quantity;
        table.Status="Đang dùng"; order.TotalAmount=(double)Total(order);
        await db.SaveChangesAsync(); await tx.CommitAsync();
        return Json(new { success=true });
    }
    public class QuantityInput
    {
        public int TableId { get; set; }
        public int OrderId { get; set; }
        public int DetailId { get; set; }
        [Range(0,99)] public int Quantity { get; set; }
    }
    [HttpPost]
    public async Task<IActionResult> Quantity([FromBody] QuantityInput input)
    {
        if (!ModelState.IsValid) return BadRequest(new { message="Số lượng phải từ 0 đến 99." });
        using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var order = await Open(input.TableId);
        var line = order?.Details.FirstOrDefault(d => d.DetailID==input.DetailId);
        if (order == null || order.OrderID!=input.OrderId || line==null) return Conflict(new { message="Đơn đã thay đổi. Hãy tải lại giỏ hàng." });
        if (input.Quantity==0) { order.Details.Remove(line); db.OrderDetails.Remove(line); }
        else line.Quantity=input.Quantity;
        order.TotalAmount=(double)Total(order);
        if (!order.Details.Any()) {
            order.Status="Đã hủy";
            var table=await db.Tables.FindAsync(input.TableId); if (table!=null) table.Status="Trống";
        }
        await db.SaveChangesAsync(); await tx.CommitAsync(); return Json(new { success=true });
    }
    public class PayInput
    {
        public int TableId { get; set; }
        public int OrderId { get; set; }
        [Required] public string Method { get; set; } = "Tiền mặt";
        [Range(typeof(decimal),"0","1000000000")] public decimal CustomerPaid { get; set; }
        [Range(typeof(decimal),"0","1000000000")] public decimal ExpectedTotal { get; set; }
        public bool Confirmed { get; set; }
    }
    [HttpPost]
    public async Task<IActionResult> Pay([FromBody] PayInput input)
    {
        if (!ModelState.IsValid || !input.Confirmed || !(new[]{"Tiền mặt","Chuyển khoản"}).Contains(input.Method))
            return BadRequest(new { message="Kiểm tra phương thức và xác nhận đã nhận tiền." });
        using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var order=await Open(input.TableId);
        if (order==null || order.OrderID!=input.OrderId || !order.Details.Any())
            return Conflict(new { message="Đơn đã được thanh toán hoặc thay đổi. Hãy tải lại." });
        var total=Total(order);
        if (total!=input.ExpectedTotal) return Conflict(new { message="Tổng đơn đã thay đổi. Hãy kiểm tra lại trước khi thu tiền." });
        if (input.Method=="Tiền mặt" && input.CustomerPaid<total) return BadRequest(new { message="Tiền khách đưa chưa đủ." });
        order.TotalAmount=(double)total;
        order.CustomerPaid=(double)(input.Method=="Tiền mặt" ? input.CustomerPaid : total);
        order.ChangeAmount=order.CustomerPaid-(double)total;
        order.PaymentMethod=input.Method; order.IsPaid=true; order.Status="Đã thanh toán";
        var table=await db.Tables.FindAsync(input.TableId); if(table!=null) table.Status="Trống";
        await db.SaveChangesAsync(); await tx.CommitAsync();
        return Json(new { success=true, receiptUrl=Url.Action("Receipt",new {id=order.OrderID}) });
    }
    public async Task<IActionResult> Receipt(int id)
    {
        var order=await db.Orders.AsNoTracking().Include(o=>o.Table).Include(o=>o.Details).ThenInclude(d=>d.Product)
            .FirstOrDefaultAsync(o=>o.OrderID==id && o.IsPaid);
        return order==null ? NotFound() : View(order);
    }
}
