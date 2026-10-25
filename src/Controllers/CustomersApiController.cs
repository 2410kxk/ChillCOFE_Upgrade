using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
namespace QuanLiQuanCaffee.Controllers;
[ApiController, Route("api/customers"), ApiExplorerSettings(GroupName="customers")]
[Authorize(Roles="Admin,Staff"), AutoValidateAntiforgeryToken]
public class CustomersApiController : ControllerBase
{
    private readonly AppDbContext db;
    public CustomersApiController(AppDbContext db) { this.db=db; }
    public class Input
    {
        [Required,StringLength(100)] public string Name {get;set;}="";
        [Required,RegularExpression(@"^[0-9+]{8,20}$")] public string Phone {get;set;}="";
        [EmailAddress,StringLength(200)] public string? Email {get;set;}
        public bool IsActive {get;set;}=true;
    }
    [HttpGet]
    public async Task<IActionResult> List(string? search=null, [Range(1,int.MaxValue)] int page=1, [Range(1,100)] int pageSize=20)
    {
        var q=db.CafeCustomers.AsNoTracking().AsQueryable();
        if(!string.IsNullOrWhiteSpace(search)) q=q.Where(c=>c.Name.Contains(search.Trim()) || c.Phone.Contains(search.Trim()));
        var offset=((long)page-1)*pageSize; if(offset>int.MaxValue) return BadRequest();
        return Ok(new {count=await q.CountAsync(),page,pageSize,items=await q.OrderBy(c=>c.CafeCustomerId).Skip((int)offset).Take(pageSize).ToListAsync()});
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var c=await db.CafeCustomers.AsNoTracking().FirstOrDefaultAsync(c=>c.CafeCustomerId==id);
        return c==null ? NotFound() : Ok(c);
    }
    [HttpPost]
    public async Task<IActionResult> Create(Input input)
    {
        if(string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new {message="Tên không được trống."});
        if(await db.CafeCustomers.AnyAsync(c=>c.Phone==input.Phone)) return Conflict(new {message="Số điện thoại đã tồn tại."});
        var c=new CafeCustomer {Name=input.Name.Trim(),Phone=input.Phone,Email=input.Email,IsActive=input.IsActive};
        db.CafeCustomers.Add(c);
        try { await db.SaveChangesAsync(); }
        catch(DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sql && (sql.Number==2601 || sql.Number==2627))
        { return Conflict(new {message="Số điện thoại đã tồn tại."}); }
        return CreatedAtAction(nameof(Get),new {id=c.CafeCustomerId},c);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Input input)
    {
        var c=await db.CafeCustomers.FindAsync(id); if(c==null) return NotFound();
        if(string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new {message="Tên không được trống."});
        if(await db.CafeCustomers.AnyAsync(x=>x.CafeCustomerId!=id && x.Phone==input.Phone)) return Conflict(new {message="Số điện thoại đã tồn tại."});
        c.Name=input.Name.Trim();c.Phone=input.Phone;c.Email=input.Email;c.IsActive=input.IsActive;
        try {await db.SaveChangesAsync();}
        catch(DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sql && (sql.Number==2601 || sql.Number==2627))
        {return Conflict(new {message="Số điện thoại đã tồn tại."});}
        return Ok(c);
    }
    [HttpDelete("{id:int}"),Authorize(Roles="Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var c=await db.CafeCustomers.FindAsync(id);if(c==null) return NotFound();
        c.IsActive=false;await db.SaveChangesAsync();return Ok(new {message="Đã ngừng sử dụng hồ sơ, giữ lịch sử mua.",c.CafeCustomerId,c.IsActive});
    }
    [HttpGet("{id:int}/orders")]
    public async Task<IActionResult> History(int id, [Range(1,int.MaxValue)] int page=1, [Range(1,100)] int pageSize=20)
    {
        if(!await db.CafeCustomers.AnyAsync(c=>c.CafeCustomerId==id)) return NotFound();
        var q=db.Orders.AsNoTracking().Where(o=>o.CafeCustomerId==id && o.IsPaid && o.Status=="Đã thanh toán");
        var offset=((long)page-1)*pageSize;if(offset>int.MaxValue) return BadRequest();
        return Ok(new {count=await q.CountAsync(),page,pageSize,items=await q.OrderByDescending(o=>o.OrderTime)
            .ThenByDescending(o=>o.OrderID).Skip((int)offset).Take(pageSize)
            .Select(o=>new {o.OrderID,o.OrderTime,o.TotalAmount,o.PaymentMethod,o.Status}).ToListAsync()});
    }
    public class LinkInput { [Range(1,int.MaxValue)] public int CustomerId {get;set;} }
    [HttpPut("/api/orders/{orderId:int}/customer")]
    public async Task<IActionResult> Link(int orderId, LinkInput input)
    {
        using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var o=await db.Orders.FindAsync(orderId);if(o==null) return NotFound();
        if(o.IsPaid || o.Status!="Đang xử lý") return Conflict(new {message="Chỉ gắn khách vào đơn đang mở."});
        var c=await db.CafeCustomers.FindAsync(input.CustomerId);
        if(c==null || !c.IsActive) return BadRequest(new {message="Khách không tồn tại hoặc đã ngừng sử dụng."});
        o.CafeCustomerId=c.CafeCustomerId;o.CustomerName=c.Name;o.Phone=c.Phone;
        await db.SaveChangesAsync();await tx.CommitAsync();return Ok(new {o.OrderID,o.CafeCustomerId});
    }
}
