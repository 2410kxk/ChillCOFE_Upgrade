using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;

namespace QuanLiQuanCaffee.Controllers;
[Authorize, AutoValidateAntiforgeryToken]
public class OnlineController : Controller
{
    private readonly AppDbContext db;
    public OnlineController(AppDbContext db) => this.db = db;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private string CartKey => "CartV2:" + UserId;
    public class Line
    {
        public string Key { get; set; } = Guid.NewGuid().ToString("N");
        public int ProductId { get; set; }
        [Range(1,99)] public int Quantity { get; set; } = 1;
        public string Size { get; set; } = "M";
        public int Sugar { get; set; } = 100;
        public int Ice { get; set; } = 100;
        public string Topping { get; set; } = "Không";
        [StringLength(200)] public string? Note { get; set; }
    }
    public class Shipping
    {
        [Required, StringLength(100)] public string Name { get; set; } = "";
        [Required, RegularExpression(@"\+?[0-9]{9,15}")] public string Phone { get; set; } = "";
        [Required, StringLength(300, MinimumLength=5)] public string Address { get; set; } = "";
        [Required] public string PaymentMethod { get; set; } = "Tiền mặt";
        [Required] public string CheckoutKey { get; set; } = "";
        public double ExpectedTotal { get; set; }
    }
    private List<Line> ReadCart() => JsonSerializer.Deserialize<List<Line>>(HttpContext.Session.GetString(CartKey) ?? "[]") ?? new();
    private void SaveCart(List<Line> lines) { HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(lines)); HttpContext.Session.Remove(CartKey+":checkout"); }
    private static readonly Dictionary<string,int> Toppings = new() { ["Không"]=0,["Trân châu"]=5000,["Thạch trái cây"]=5000,["Kem cheese"]=10000 };
    private static bool Valid(Line line) => line.Quantity is >=1 and <=99 && new[]{"M","L"}.Contains(line.Size) && new[]{0,25,50,75,100,125}.Contains(line.Sugar) && new[]{0,25,50,75,100,125}.Contains(line.Ice) && Toppings.ContainsKey(line.Topping ?? "") && (line.Note?.Length ?? 0)<=200;
    private static double Price(Product p, Line l) => p.Price + (p.SupportsCustomization ? (l.Size=="L"?7000:0)+Toppings[l.Topping] : 0);
    private static void Normalize(Product p,Line l) { if(!p.SupportsCustomization){l.Size="M";l.Sugar=100;l.Ice=100;l.Topping="Không";} l.Note=l.Note?.Trim(); }
    public IActionResult Index() => View();
    [HttpGet]
    public async Task<IActionResult> Cart()
    {
        var lines=ReadCart();var ids=lines.Select(l=>l.ProductId).ToList();
        var products=await db.Products.AsNoTracking().Where(p=>ids.Contains(p.ProductID)).ToDictionaryAsync(p=>p.ProductID);
        var token=HttpContext.Session.GetString(CartKey+":checkout") ?? Guid.NewGuid().ToString();
        HttpContext.Session.SetString(CartKey+":checkout",token);
        var items=lines.Select(l=> {products.TryGetValue(l.ProductId,out var p);return new {l.Key,l.ProductId,l.Quantity,l.Size,l.Sugar,l.Ice,l.Topping,l.Note,name=p?.Name ?? "Món đã xóa",image=p?.ImageUrl,available=p?.IsAvailable==true,custom=p?.SupportsCustomization==true,price=p==null?0:Price(p,l)};}).ToList();
        return Json(new {items,count=lines.Sum(l=>l.Quantity),total=items.Sum(l=>l.price*l.Quantity),checkoutKey=token});
    }
    [HttpPost]
    public async Task<IActionResult> Add(int productId)
    {
        var p=await db.Products.FindAsync(productId);if(p==null||!p.IsAvailable)return BadRequest(new{message="Món đang tạm hết."});
        var lines=ReadCart();if(lines.Count>=100)return BadRequest(new{message="Giỏ hàng tối đa 100 dòng."});
        var line=lines.FirstOrDefault(l=>l.ProductId==productId&&l.Size=="M"&&l.Topping=="Không"&&l.Sugar==100&&l.Ice==100&&string.IsNullOrEmpty(l.Note));
        if(line==null)lines.Add(new Line{ProductId=productId});
        else if(line.Quantity<99)line.Quantity++;else return BadRequest(new{message="Tối đa 99 món mỗi dòng."});
        SaveCart(lines);return Json(new{count=lines.Sum(l=>l.Quantity),message="Đã thêm vào giỏ hàng"});
    }
    [HttpPost]
    public async Task<IActionResult> Update(Line input)
    {
        if(!ModelState.IsValid||!Valid(input))return BadRequest(new{message="Kiểm tra số lượng, size, đường, đá, topping và ghi chú (tối đa 200 ký tự)."});
        var lines=ReadCart();var stored=lines.FirstOrDefault(l=>l.Key==input.Key);if(stored==null)return NotFound();
        var p=await db.Products.FindAsync(stored.ProductId);if(p==null)return NotFound();
        input.ProductId=stored.ProductId;Normalize(p,input);lines[lines.IndexOf(stored)]=input;SaveCart(lines);return Json(new{success=true});
    }
    [HttpPost]
    public IActionResult Remove(string key) {var lines=ReadCart();lines.RemoveAll(l=>l.Key==key);SaveCart(lines);return Json(new{success=true});}
    [HttpPost]
    public async Task<IActionResult> Checkout(Shipping input)
    {
        if(!ModelState.IsValid || !Guid.TryParse(input.CheckoutKey,out _) || !new[]{"Tiền mặt","Chuyển khoản"}.Contains(input.PaymentMethod))return BadRequest(new{message="Nhập đủ tên, số điện thoại, địa chỉ và phương thức thanh toán hợp lệ."});
        var existing=await db.Orders.AsNoTracking().FirstOrDefaultAsync(o=>o.CheckoutKey==input.CheckoutKey&&o.UserId==UserId);
        if(existing!=null)return Json(new{url="/Online/History",orderId=existing.OrderID});
        if(input.CheckoutKey!=HttpContext.Session.GetString(CartKey+":checkout"))return Conflict(new{message="Giỏ hàng đã thay đổi. Tải lại và kiểm tra trước khi đặt."});
        var lines=ReadCart();if(lines.Count==0)return BadRequest(new{message="Giỏ hàng đang trống."});
        await using var tx=await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var ids=lines.Select(l=>l.ProductId).ToList();var products=await db.Products.Where(p=>ids.Contains(p.ProductID)).ToDictionaryAsync(p=>p.ProductID);
        if(lines.Any(l=>!Valid(l)||!products.TryGetValue(l.ProductId,out var p)||!p.IsAvailable))return Conflict(new{message="Có món không còn phục vụ. Kiểm tra lại giỏ hàng."});
        foreach(var l in lines)Normalize(products[l.ProductId],l);
        var total=lines.Sum(l=>Price(products[l.ProductId],l)*l.Quantity);
        if(!double.IsFinite(input.ExpectedTotal)||Math.Abs(total-input.ExpectedTotal)>.01)return Conflict(new{message="Giá đã thay đổi. Tải lại giỏ hàng để xác nhận giá mới."});
        var order=new Order{IsOnline=true,UserId=UserId,Status="Chờ xác nhận",OrderTime=DateTime.Now,CustomerName=input.Name.Trim(),Phone=input.Phone.Trim(),Address=input.Address.Trim(),PaymentMethod=input.PaymentMethod,CheckoutKey=input.CheckoutKey,TotalAmount=total,
            Details=lines.Select(l=>new OrderDetail{ProductID=l.ProductId,Quantity=l.Quantity,Size=l.Size,Sugar=l.Sugar,Ice=l.Ice,Topping=l.Topping,Note=l.Note??"",Price=Price(products[l.ProductId],l)}).ToList()};
        db.Orders.Add(order);
        try {await db.SaveChangesAsync();await tx.CommitAsync();}
        catch(DbUpdateException){await tx.RollbackAsync();return Conflict(new{message="Yêu cầu có thể đã được xử lý. Kiểm tra lịch sử trước khi đặt lại."});}
        SaveCart(new());return Json(new{url="/Online/History",orderId=order.OrderID});
    }
    public async Task<IActionResult> History() => View(await db.Orders.AsNoTracking().Include(o=>o.Details).ThenInclude(d=>d.Product).Where(o=>o.UserId==UserId&&o.IsOnline).OrderByDescending(o=>o.OrderTime).ToListAsync());
    [Authorize(Roles="Admin,Staff")]
    public async Task<IActionResult> Deliveries() {ViewData["Workspace"]=true;return View(await db.Orders.AsNoTracking().Include(o=>o.Details).ThenInclude(d=>d.Product).Where(o=>o.IsOnline).OrderByDescending(o=>o.OrderTime).ToListAsync());}
    [Authorize(Roles="Admin,Staff"),HttpPost]
    public async Task<IActionResult> Transition(int id,string actionName,bool paymentConfirmed=false)
    {
        // Conditional SQL update is atomic: repeated clicks cannot book revenue twice.
        var q=db.Orders.Where(o=>o.OrderID==id&&o.IsOnline&&!o.IsPaid);int changed=0;
        if(actionName=="ship")changed=await q.Where(o=>o.Status=="Chờ xác nhận").ExecuteUpdateAsync(s=>s.SetProperty(o=>o.Status,"Đang giao"));
        else if(actionName=="cancel")changed=await q.Where(o=>o.Status=="Chờ xác nhận"||o.Status=="Đang giao").ExecuteUpdateAsync(s=>s.SetProperty(o=>o.Status,"Đã hủy"));
        else if(actionName=="complete"&&paymentConfirmed){var now=DateTime.Now;changed=await q.Where(o=>o.Status=="Đang giao").ExecuteUpdateAsync(s=>s.SetProperty(o=>o.Status,"Đã thanh toán").SetProperty(o=>o.IsPaid,true).SetProperty(o=>o.PaidAt,now).SetProperty(o=>o.DeliveredAt,now).SetProperty(o=>o.CustomerPaid,o=>o.TotalAmount));}
        TempData[changed==1?"Success":"Error"]=changed==1?"Đã cập nhật đơn hàng.":"Trạng thái đã thay đổi hoặc chưa xác nhận nhận đủ tiền. Tải lại danh sách để kiểm tra.";
        return RedirectToAction(nameof(Deliveries));
    }
    [Authorize(Roles="Admin,Staff"),HttpPost]
    public async Task<IActionResult> TableNote(int id,string? note)
    {
        if((note?.Length??0)>300)return BadRequest("Ghi chú tối đa 300 ký tự.");
        var table=await db.Tables.FindAsync(id);if(table==null)return NotFound();table.Note=note?.Trim();await db.SaveChangesAsync();return RedirectToAction("Tables","Pos");
    }
}
