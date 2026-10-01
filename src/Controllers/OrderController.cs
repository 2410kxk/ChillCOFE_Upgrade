    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using QuanLiQuanCaffee.Models;

    [Authorize(Roles = "Admin,Staff,User")]
    public class OrderController : BaseController
    {
        private new  readonly AppDbContext _context;

        public OrderController(AppDbContext context) : base(context)
        {
            _context = context;
        }

    // =========================
    // 🪑 DANH SÁCH BÀN
    // =========================
    [Authorize(Roles = "Admin,Staff")]
    public IActionResult Tables()
        { return RedirectToAction("Tables", "Pos"); }
        //khach online
        [HttpPost]
        // Thêm các hàm hỗ trợ vào cuối Class OrderController
        // 1. Lấy giỏ hàng từ Session
        private List<OrderDetail> GetOnlineCart()
        {
            // Gọi hàm Get<T> chúng ta vừa viết ở Bước 1
            var cart = HttpContext.Session.Get<List<OrderDetail>>("OnlineCart");
            return cart ?? new List<OrderDetail>();
        }

        // 2. Lưu giỏ hàng vào Session
        private void SaveOnlineCart(List<OrderDetail> cart)
        {
            // Gọi hàm Set<T> chúng ta vừa viết ở Bước 1
            HttpContext.Session.Set("OnlineCart", cart);
        }
        // Action lấy danh sách món đã chọn tại bàn (Dùng cho cột bên phải)
        [Authorize(Roles = "Admin,Staff")]
public IActionResult GetCartSidebar()
        {
            int? tableId = HttpContext.Session.GetInt32("Table");
            var order = _context.Orders
                .Include(o => o.Details).ThenInclude(d => d.Product)
                .FirstOrDefault(o => o.TableID == tableId && o.Status == "Đang xử lý");

            return PartialView("_CartSidebar", order?.Details.ToList() ?? new List<OrderDetail>());
        }
    [Authorize(Roles = "Admin")]
    public IActionResult Manage()
    {
        var orders = _context.Orders.OrderByDescending(o => o.OrderTime).ToList();
        return View(orders);
    }
    // Action cho khách mua Online
    [HttpPost]
    public IActionResult AddToOnlineCart(int id)
    {
        var product = _context.Products.Find(id);

        if (product == null || !product.IsAvailable)
            return Json(new { success = false, message = "Món hiện không phục vụ" });

        var cart = GetOnlineCart();
        var item = cart.FirstOrDefault(x => x.ProductID == id);

        if (item == null)
        {
            cart.Add(new OrderDetail
            {
                ProductID = id,
                Quantity = 1,
                Price = product.Price
                // ❌ KHÔNG lưu Product
            });
        }
        else
        {
            item.Quantity++;
        }

        SaveOnlineCart(cart);

        return Json(new { count = cart.Sum(x => x.Quantity) });
    }
    //Bạn cần một Action trả về Partial View chứa danh sách món để AJAX nạp vào:
    [Authorize(Roles = "Admin,Staff")]
public IActionResult GetCartPartial()
        {
            var tableId = HttpContext.Session.GetInt32("Table");
            // Lấy Order của bàn hiện tại từ Database
            var order = _context.Orders
                .Include(o => o.Details).ThenInclude(d => d.Product)
                .FirstOrDefault(o => o.TableID == tableId && o.Status == "Đang xử lý");

            var details = order?.Details.ToList() ?? new List<OrderDetail>();
            return PartialView("_CartSidebar", details);
        }


        // =========================
        // 👉 CHỌN BÀN + HIỂN THỊ MENU
        // =========================
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Menu(int tableId)
        { return RedirectToAction("Index", "Pos", new { tableId }); }

        // =========================
        // ➕ THÊM MÓN (AJAX)
        // =========================
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
[ValidateAntiForgeryToken]
    public IActionResult AddToCart(int id)
        { return StatusCode(410, new { message="Vui lòng sử dụng màn hình bán hàng Chill COFÉ mới tại /Pos/Tables." }); }
        // =========================
        // 🛒 GIỎ HÀNG
        // =========================
        [Authorize(Roles = "Admin,Staff")]
public IActionResult Cart()
        {
            int? tableId = HttpContext.Session.GetInt32("Table");

            if (tableId == null)
                return RedirectToAction("Tables");

            var order = _context.Orders
                .Include(o => o.Details)
                .ThenInclude(d => d.Product)
                .FirstOrDefault(o => o.TableID == tableId && o.Status == "Đang xử lý");

            if (order == null)
                return View(new List<OrderDetail>());

            return View(order.Details.ToList());
        }
    public IActionResult OnlineCart()
    {
        var cart = GetOnlineCart();

        var productIds = cart.Select(x => x.ProductID).ToList();

        var products = _context.Products
            .Where(p => productIds.Contains(p.ProductID))
            .ToList();

        foreach (var item in cart)
        {
            item.Product = products.FirstOrDefault(p => p.ProductID == item.ProductID);
        }

        return View(cart);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(int id, int delta)
    {
        int? tableId = null;

        if (delta != 1 && delta != -1) return BadRequest();
        if (tableId != null)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Staff")) return Forbid();
            // LOGIC CHO TẠI BÀN (Bạn đã có)
            var order = _context.Orders
                .FirstOrDefault(o => o.TableID == tableId && o.Status == "Đang xử lý");
            if (order != null)
            {
                var detail = _context.OrderDetails
                    .FirstOrDefault(d => d.OrderID == order.OrderID && d.ProductID == id);
                if (detail != null)
                {
                    detail.Quantity += delta;
                    if (detail.Quantity <= 0) _context.OrderDetails.Remove(detail);
                    _context.SaveChanges();
                }
            }
        }
        else
        {
            // LOGIC CHO ONLINE
            var cart = GetOnlineCart();
            var item = cart.FirstOrDefault(x => x.ProductID == id);
            if (item != null)
            {
                item.Quantity += delta;
                if (item.Quantity <= 0) cart.Remove(item);
                SaveOnlineCart(cart); // Lưu lại vào Session
            }
        }

        return Json(new { success = true });
    }
    // =========================
    // ➕ TĂNG SỐ LƯỢNG
    // =========================
    [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
[ValidateAntiForgeryToken]
    public IActionResult Increase(int detailId)
        { return StatusCode(410, new { message="Vui lòng sử dụng màn hình bán hàng Chill COFÉ mới tại /Pos/Tables." }); }

        // =========================
        // ➖ GIẢM SỐ LƯỢNG
        // =========================
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
[ValidateAntiForgeryToken]
    public IActionResult Decrease(int detailId)
        { return StatusCode(410, new { message="Vui lòng sử dụng màn hình bán hàng Chill COFÉ mới tại /Pos/Tables." }); }

        // =========================
        // ❌ XÓA MÓN
        // =========================
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
[ValidateAntiForgeryToken]
    public IActionResult Remove(int detailId)
        { return StatusCode(410, new { message="Vui lòng sử dụng màn hình bán hàng Chill COFÉ mới tại /Pos/Tables." }); }

        // =========================
        // 💳 TRANG THANH TOÁN
        // =========================
        [Authorize(Roles = "Admin,Staff")]
public IActionResult Checkout()
        {
            var tableId = HttpContext.Session.GetInt32("Table");

            if (tableId == null)
                return RedirectToAction("Tables");

            var order = _context.Orders
                .Include(o => o.Details)
                .ThenInclude(d => d.Product)
                .FirstOrDefault(o => o.TableID == tableId && o.Status == "Đang xử lý");

            if (order == null)
                return RedirectToAction("Cart");

            return View(order);
        }

        // =========================
        // ✅ XÁC NHẬN THANH TOÁN
        // =========================
        [HttpPost]
[Authorize(Roles = "Admin,Staff")]
[ValidateAntiForgeryToken]
    public IActionResult ConfirmCheckout(string paymentMethod = "Tiền mặt")
{ return StatusCode(410, new { message="Vui lòng sử dụng màn hình bán hàng Chill COFÉ mới tại /Pos/Tables." }); }

        // =========================
        // 🔢 SỐ LƯỢNG GIỎ HÀNG
        // =========================
        public IActionResult GetCartCount()
        {
            var tableId = HttpContext.Session.GetInt32("Table");

            if (tableId == null)
                return Json(0);

            var count = _context.OrderDetails
                .Where(d => d.Order.TableID == tableId && d.Order.Status == "Đang xử lý")
                .Sum(d => d.Quantity);

            return Json(count);
        }
    public IActionResult OnlineCheckout()
    {
        return View();
    }
    // Action lấy chi tiết đơn hàng để hiển thị lên Modal (Dùng AJAX)
    // Action lấy chi tiết đơn hàng để hiển thị lên Modal (Dùng AJAX)
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
public IActionResult GetOrderDetailsJson(int id)
    {
        // Sử dụng .AsNoTracking() để tăng tốc độ truy vấn và tránh lỗi vòng lặp
        var order = _context.Orders
            .Include(o => o.Details)
            .ThenInclude(d => d.Product)
            .FirstOrDefault(o => o.OrderID == id);

        if (order == null)
        {
            return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
        }

        // Trả về dữ liệu sạch để JavaScript dễ xử lý
        var result = new
        {
            orderId = order.OrderID,
            // Kiểm tra logic hiển thị tên khách hoặc bàn
            customer = order.Status == "Online" ? $"{order.CustomerName} ({order.Phone})" : $"Bàn {order.TableID}",
            time = order.OrderTime.ToString("dd/MM/yyyy HH:mm"),
            address = order.Address ?? "Tại quán",
            status = order.Status,
            details = order.Details.Select(d => new
            {
                // Kiểm tra nếu Product bị null thì hiện "Sản phẩm đã xóa"
                productName = d.Product != null ? d.Product.Name : "Sản phẩm không tồn tại",
                quantity = d.Quantity,
                price = d.Price,
                total = d.Quantity * d.Price
            }).ToList(),
            totalAmount = order.Details.Sum(d => d.Quantity * d.Price)
        };

        return Json(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmOnlinePayment(int orderId)
    {
        var order = _context.Orders.Include(o => o.Details).FirstOrDefault(o => o.OrderID == orderId && o.Status == "Online" && !o.IsPaid);

        if (order != null)
        {
            // Cập nhật trạng thái để AdminController có thể tính vào doanh thu
            order.TotalAmount = order.Details.Sum(d => d.Quantity * d.Price);
            order.IsPaid = true;
            order.Status = "Đã thanh toán";
            _context.SaveChanges();

            return Json(new { success = true });
        }

        return Json(new { success = false, message = "Không tìm thấy đơn hàng hoặc trạng thái không hợp lệ." });
    }

    [HttpPost]
    public IActionResult PlaceOnlineOrder(string name, string phone, string address)
    {
        var cart = GetOnlineCart();

        if (cart == null || !cart.Any())
            return Json(new { success = false });

        var order = new Order
        {
            OrderTime = DateTime.Now,
            Status = "Online",
            CustomerName = name,
            Phone = phone,
            Address = address
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        foreach (var item in cart)
        {
            _context.OrderDetails.Add(new OrderDetail
            {
                OrderID = order.OrderID,
                ProductID = item.ProductID,
                Quantity = item.Quantity,
                Price = item.Price
            });
        }

        _context.SaveChanges();

        // 🔥 XÓA GIỎ ONLINE
        HttpContext.Session.Remove("OnlineCart");

        return Json(new { success = true });
    }
}

