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

    public PosController(AppDbContext db)
    {
        this.db = db;
    }

    // ==================== GIAO DIỆN ====================

    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Tables()
    {
        var tables = await db.Tables
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync();

        return View(tables);
    }

    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Index(int tableId)
    {
        var table = await db.Tables
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TableID == tableId);

        if (table == null)
        {
            return NotFound();
        }

        var model = new PosVm
        {
            Table = table,

            Products = await db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .OrderBy(p => p.CategoryID)
                .ThenBy(p => p.Name)
                .ToListAsync(),

            Categories = await db.Categories
                .AsNoTracking()
                .OrderBy(c => c.CategoryID)
                .ToListAsync()
        };

        return View(model);
    }

    // ==================== HÀM HỖ TRỢ ====================

    private Task<Order?> Open(int tableId)
    {
        return db.Orders
            .Include(o => o.Details)
            .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o =>
                o.TableID == tableId &&
                o.Status == "Đang xử lý" &&
                !o.IsPaid);
    }

    private static decimal Total(Order order)
    {
        return order.Details.Sum(d =>
            (decimal)d.Price * d.Quantity);
    }

    // ==================== API XEM GIỎ HÀNG ====================

    [HttpGet("/Pos/Cart")]
    [ApiExplorerSettings(GroupName = "pos")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cart([FromQuery] int tableId)
    {
        if (!ModelState.IsValid || tableId <= 0)
        {
            return BadRequest(new
            {
                message = "Mã bàn không hợp lệ."
            });
        }

        var tableExists = await db.Tables
            .AnyAsync(t => t.TableID == tableId);

        if (!tableExists)
        {
            return NotFound(new
            {
                message = "Bàn không tồn tại."
            });
        }

        var order = await Open(tableId);

        return Json(new
        {
            orderId = order?.OrderID ?? 0,
            total = order == null ? 0m : Total(order),

            items = order?.Details.Select(d => new
            {
                id = d.DetailID,
                name = d.Product?.Name ?? "Món đã lưu",
                quantity = d.Quantity,
                price = d.Price,
                d.Size,
                d.Sugar,
                d.Ice,
                d.Topping,
                d.Note
            }).ToArray()
        });
    }

    // ==================== DỮ LIỆU THÊM MÓN ====================

    public class AddInput
    {
        [Range(1, int.MaxValue)]
        public int TableId { get; set; }

        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, 99)]
        public int Quantity { get; set; } = 1;

        [Required]
        public string Size { get; set; } = "M";

        public int Sugar { get; set; } = 100;

        public int Ice { get; set; } = 100;

        [Required]
        public string Topping { get; set; } = "Không";

        [MaxLength(200)]
        public string? Note { get; set; }
    }

    // ==================== API THÊM MÓN ====================

    [HttpPost("/Pos/Add")]
    [ApiExplorerSettings(GroupName = "pos")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] AddInput? input)
    {
        if (input == null || !ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Thông tin món không hợp lệ."
            });
        }

        using var tx = await db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable);

        var table = await db.Tables.FindAsync(input.TableId);
        var product = await db.Products.FindAsync(input.ProductId);

        if (table == null || product == null || !product.IsAvailable)
        {
            return BadRequest(new
            {
                message = "Bàn hoặc món không tồn tại, hoặc món đã hết."
            });
        }

        if (!double.IsFinite(product.Price) ||
            product.Price < 0 ||
            product.Price > 10000000)
        {
            return BadRequest(new
            {
                message = "Giá món chưa hợp lệ."
            });
        }

        // Món không hỗ trợ tùy chỉnh dùng các giá trị mặc định.
        if (!product.SupportsCustomization)
        {
            input.Size = "M";
            input.Sugar = 100;
            input.Ice = 100;
            input.Topping = "Không";
        }

        var toppingPrices = new Dictionary<string, decimal>
        {
            ["Không"] = 0m,
            ["Trân châu"] = 5000m,
            ["Thạch trái cây"] = 5000m,
            ["Kem cheese"] = 10000m
        };

        var validSizes = new[] { "M", "L" };
        var validSugarLevels = new[] { 0, 30, 50, 70, 100 };
        var validIceLevels = new[] { 0, 50, 100 };

        if (!validSizes.Contains(input.Size) ||
            !validSugarLevels.Contains(input.Sugar) ||
            !validIceLevels.Contains(input.Ice) ||
            string.IsNullOrWhiteSpace(input.Topping) ||
            !toppingPrices.ContainsKey(input.Topping))
        {
            return BadRequest(new
            {
                message = "Tùy chọn món không hợp lệ."
            });
        }

        // Giá được tính tại server.
        var unitPrice =
            (decimal)product.Price +
            (input.Size == "L" ? 7000m : 0m) +
            toppingPrices[input.Topping];

        var order = await Open(input.TableId);

        if (order == null)
        {
            order = new Order
            {
                TableID = input.TableId,
                Status = "Đang xử lý",
                OrderTime = DateTime.Now
            };

            db.Orders.Add(order);
        }

        var note = input.Note?.Trim() ?? "";

        var line = order.Details.FirstOrDefault(d =>
            d.ProductID == input.ProductId &&
            d.Size == input.Size &&
            d.Sugar == input.Sugar &&
            d.Ice == input.Ice &&
            d.Topping == input.Topping &&
            d.Note == note &&
            (decimal)d.Price == unitPrice);

        if (line != null && line.Quantity + input.Quantity > 99)
        {
            return BadRequest(new
            {
                message = "Tối đa 99 phần cho một dòng món."
            });
        }

        if (line == null)
        {
            order.Details.Add(new OrderDetail
            {
                ProductID = input.ProductId,
                Quantity = input.Quantity,
                Price = (double)unitPrice,
                Size = input.Size,
                Sugar = input.Sugar,
                Ice = input.Ice,
                Topping = input.Topping,
                Note = note
            });
        }
        else
        {
            line.Quantity += input.Quantity;
        }

        table.Status = "Đang dùng";
        order.TotalAmount = (double)Total(order);

        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return Json(new
        {
            success = true
        });
    }

    // ==================== DỮ LIỆU ĐỔI SỐ LƯỢNG ====================

    public class QuantityInput
    {
        [Range(1, int.MaxValue)]
        public int TableId { get; set; }

        [Range(1, int.MaxValue)]
        public int OrderId { get; set; }

        [Range(1, int.MaxValue)]
        public int DetailId { get; set; }

        [Range(0, 99)]
        public int Quantity { get; set; }
    }

    // ==================== API ĐỔI SỐ LƯỢNG ====================

    [HttpPost("/Pos/Quantity")]
    [ApiExplorerSettings(GroupName = "pos")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Quantity(
        [FromBody] QuantityInput? input)
    {
        if (input == null || !ModelState.IsValid)
        {
            return BadRequest(new
            {
                message =
                    "Mã bàn, đơn hoặc dòng món không hợp lệ; " +
                    "số lượng phải từ 0 đến 99."
            });
        }

        using var tx = await db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable);

        var order = await Open(input.TableId);

        var line = order?.Details.FirstOrDefault(d =>
            d.DetailID == input.DetailId);

        if (order == null ||
            order.OrderID != input.OrderId ||
            line == null)
        {
            return Conflict(new
            {
                message = "Đơn đã thay đổi. Hãy tải lại giỏ hàng."
            });
        }

        if (input.Quantity == 0)
        {
            order.Details.Remove(line);
            db.OrderDetails.Remove(line);
        }
        else
        {
            line.Quantity = input.Quantity;
        }

        order.TotalAmount = (double)Total(order);

        // Xóa hết món thì hủy đơn và giải phóng bàn.
        if (!order.Details.Any())
        {
            order.Status = "Đã hủy";

            var table = await db.Tables.FindAsync(input.TableId);

            if (table != null)
            {
                table.Status = "Trống";
            }
        }

        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return Json(new
        {
            success = true
        });
    }

    // ==================== DỮ LIỆU THANH TOÁN ====================

    public class PayInput
    {
        [Range(1, int.MaxValue)]
        public int TableId { get; set; }

        [Range(1, int.MaxValue)]
        public int OrderId { get; set; }

        [Required]
        public string Method { get; set; } = "Tiền mặt";

        [Range(typeof(decimal), "0", "1000000000")]
        public decimal CustomerPaid { get; set; }

        [Range(typeof(decimal), "0", "1000000000")]
        public decimal ExpectedTotal { get; set; }

        public bool Confirmed { get; set; }
    }

    // ==================== API THANH TOÁN ====================

    [HttpPost("/Pos/Pay")]
    [ApiExplorerSettings(GroupName = "pos")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Pay([FromBody] PayInput? input)
    {
        if (input == null || !ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Dữ liệu thanh toán không hợp lệ."
            });
        }

        var validMethods = new[] { "Tiền mặt", "Chuyển khoản" };

        if (!input.Confirmed || !validMethods.Contains(input.Method))
        {
            return BadRequest(new
            {
                message =
                    "Kiểm tra phương thức và xác nhận đã nhận tiền."
            });
        }

        using var tx = await db.Database.BeginTransactionAsync(
            IsolationLevel.Serializable);

        var order = await Open(input.TableId);

        if (order == null ||
            order.OrderID != input.OrderId ||
            !order.Details.Any())
        {
            return Conflict(new
            {
                message =
                    "Đơn đã được thanh toán hoặc thay đổi. Hãy tải lại."
            });
        }

        var total = Total(order);

        if (total != input.ExpectedTotal)
        {
            return Conflict(new
            {
                message =
                    "Tổng đơn đã thay đổi. " +
                    "Hãy kiểm tra lại trước khi thu tiền."
            });
        }

        if (input.Method == "Tiền mặt" &&
            input.CustomerPaid < total)
        {
            return BadRequest(new
            {
                message = "Tiền khách đưa chưa đủ."
            });
        }

        var amountReceived = input.Method == "Tiền mặt"
            ? input.CustomerPaid
            : total;

        order.TotalAmount = (double)total;
        order.CustomerPaid = (double)amountReceived;
        order.ChangeAmount = (double)(amountReceived - total);
        order.PaymentMethod = input.Method;
        order.IsPaid = true;
        order.PaidAt = DateTime.Now;
        order.Status = "Đã thanh toán";

        var table = await db.Tables.FindAsync(input.TableId);

        if (table != null)
        {
            table.Status = "Trống";
        }

        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return Json(new
        {
            success = true,
            receiptUrl = Url.Action(
                nameof(Receipt),
                new { id = order.OrderID })
        });
    }

    public class CreateOrderInput
    {
        [Range(1,int.MaxValue)] public int TableId {get;set;}
    }

    [HttpPost("/api/orders")]
    [ApiExplorerSettings(GroupName="pos")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderInput? input)
    {
        if(input==null || !ModelState.IsValid) return BadRequest(new {message="Mã bàn không hợp lệ."});
        using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var t=await db.Tables.FindAsync(input.TableId);if(t==null) return NotFound(new {message="Bàn không tồn tại."});
        if(await Open(input.TableId)!=null) return Conflict(new {message="Bàn đã có đơn đang mở."});
        var o=new Order {TableID=input.TableId,Status="Đang xử lý",OrderTime=DateTime.Now};
        db.Orders.Add(o);t.Status="Đang dùng";await db.SaveChangesAsync();await tx.CommitAsync();
        return StatusCode(201,new {orderId=o.OrderID,o.TableID,o.Status});
    }

    [HttpPost("/api/orders/{id:int}/cancel")]
    [ApiExplorerSettings(GroupName="pos")]
    public async Task<IActionResult> CancelEmpty(int id)
    {
        using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var o=await db.Orders.Include(o=>o.Details).FirstOrDefaultAsync(o=>o.OrderID==id);
        if(o==null) return NotFound();
        if(o.IsPaid || o.Status!="Đang xử lý" || o.Details.Any()) return Conflict(new {message="Chỉ hủy đơn đang mở và rỗng. Xóa món trước nếu cần."});
        o.Status="Đã hủy";o.TotalAmount=0;
        if(o.TableID.HasValue) {var t=await db.Tables.FindAsync(o.TableID.Value);if(t!=null)t.Status="Trống";}
        await db.SaveChangesAsync();await tx.CommitAsync();return Ok(new {success=true});
    }

    public class EditLineInput
    {
        [Range(1,99)] public int Quantity {get;set;}=1;
        [Required] public string Size {get;set;}="M";
        public int Sugar {get;set;}=100;
        public int Ice {get;set;}=100;
        [Required] public string Topping {get;set;}="Không";
        [MaxLength(200)] public string? Note {get;set;}
    }

    [HttpPut("/api/orders/{orderId:int}/items/{detailId:int}")]
    [ApiExplorerSettings(GroupName="pos")]
    public async Task<IActionResult> EditLine(int orderId,int detailId,[FromBody] EditLineInput? input)
    {
        if(input==null || !ModelState.IsValid) return BadRequest(new {message="Dữ liệu món không hợp lệ."});
        using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var o=await db.Orders.Include(o=>o.Details).ThenInclude(d=>d.Product).FirstOrDefaultAsync(o=>o.OrderID==orderId);
        if(o==null) return NotFound();
        if(o.IsPaid || o.Status!="Đang xử lý") return Conflict(new {message="Đơn đã đóng."});
        var d=o.Details.FirstOrDefault(d=>d.DetailID==detailId);if(d==null) return NotFound();
        var p=d.Product;
        if(p==null || !p.IsAvailable || !double.IsFinite(p.Price) || p.Price<0 || p.Price>10000000)
            return BadRequest(new {message="Món không còn bán hoặc giá không hợp lệ."});
        if(!p.SupportsCustomization) {input.Size="M";input.Sugar=100;input.Ice=100;input.Topping="Không";}
        var prices=new Dictionary<string,decimal>{{"Không",0},{"Trân châu",5000},{"Thạch trái cây",5000},{"Kem cheese",10000}};
        if(!(new[]{"M","L"}).Contains(input.Size) || !(new[]{0,30,50,70,100}).Contains(input.Sugar) ||
            !(new[]{0,50,100}).Contains(input.Ice) || string.IsNullOrWhiteSpace(input.Topping) || !prices.ContainsKey(input.Topping))
            return BadRequest(new {message="Tùy chọn không hợp lệ."});
        d.Quantity=input.Quantity;d.Size=input.Size;d.Sugar=input.Sugar;d.Ice=input.Ice;d.Topping=input.Topping;d.Note=input.Note?.Trim()??"";
        d.Price=(double)((decimal)p.Price+(input.Size=="L"?7000m:0m)+prices[input.Topping]);
        o.TotalAmount=(double)Total(o);await db.SaveChangesAsync();await tx.CommitAsync();
        return Ok(new {success=true,total=o.TotalAmount,message="Dòng món được tính lại theo giá menu hiện tại."});
    }

    // ==================== GIAO DIỆN PHIẾU THANH TOÁN ====================

    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Receipt(int id)
    {
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Table)
            .Include(o => o.Details)
            .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o =>
                o.OrderID == id &&
                o.IsPaid);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}