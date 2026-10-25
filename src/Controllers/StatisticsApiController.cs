using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
namespace QuanLiQuanCaffee.Controllers;
[ApiController,Route("api/statistics"),ApiExplorerSettings(GroupName="statistics")]
[Authorize(Roles="Admin")]
public class StatisticsApiController : ControllerBase
{
    private readonly AppDbContext db;
    public StatisticsApiController(AppDbContext db) {this.db=db;}
    private static bool Valid(DateTime from,DateTime to) => from<to && (to-from).TotalDays<=366;
    private IQueryable<Order> InRange(DateTime from,DateTime to) => db.Orders.AsNoTracking().Where(o=>(o.PaidAt ?? o.OrderTime)>=from && (o.PaidAt ?? o.OrderTime)<to);
    [HttpGet("revenue")]
    public async Task<IActionResult> Revenue([FromQuery,Required] DateTime? from,[FromQuery,Required] DateTime? to)
    {
        if(!from.HasValue || !to.HasValue || !Valid(from.Value,to.Value)) return BadRequest(new {message="Cần from < to, tối đa 366 ngày; to không bao gồm."});
        var q=InRange(from.Value,to.Value).Where(o=>o.IsPaid && o.Status=="Đã thanh toán");
        var revenue=await q.SumAsync(o=>(decimal?)o.TotalAmount) ?? 0m;
        return Ok(new {from,to,revenue,currency="VND",timeBasis="PaidAt: thời điểm thu tiền; đơn cũ dùng OrderTime",paidOrders=await q.CountAsync()});
    }
    [HttpGet("orders")]
    public async Task<IActionResult> Orders([FromQuery,Required] DateTime? from,[FromQuery,Required] DateTime? to)
    {
        if(!from.HasValue || !to.HasValue || !Valid(from.Value,to.Value)) return BadRequest(new {message="Khoảng thời gian không hợp lệ, tối đa 366 ngày."});
        var q=InRange(from.Value,to.Value);
        return Ok(new {from,to,total=await q.CountAsync(),paid=await q.CountAsync(o=>o.IsPaid && o.Status=="Đã thanh toán"),
            open=await q.CountAsync(o=>!o.IsPaid && (o.Status=="Đang xử lý"||o.Status=="Chờ xác nhận"||o.Status=="Đang giao")),cancelled=await q.CountAsync(o=>o.Status=="Đã hủy")});
    }
    [HttpGet("best-sellers")]
    public async Task<IActionResult> Best([FromQuery,Required] DateTime? from,[FromQuery,Required] DateTime? to,[Range(1,50)] int top=5)
    {
        if(!from.HasValue || !to.HasValue || !Valid(from.Value,to.Value)) return BadRequest(new {message="Khoảng thời gian không hợp lệ, tối đa 366 ngày."});
        var ids=InRange(from.Value,to.Value).Where(o=>o.IsPaid && o.Status=="Đã thanh toán").Select(o=>o.OrderID);
        var items=await db.OrderDetails.AsNoTracking().Where(d=>ids.Contains(d.OrderID))
            .GroupBy(d=>new {d.ProductID,Name=d.Product==null ? "Món đã lưu" : d.Product.Name})
            .Select(g=>new {g.Key.ProductID,g.Key.Name,quantity=g.Sum(d=>(long)d.Quantity),grossAmount=g.Sum(d=>(decimal)d.Price*d.Quantity)})
            .OrderByDescending(x=>x.quantity).ThenBy(x=>x.ProductID).Take(top).ToListAsync();
        return Ok(new {from,to,top,items});
    }
    [HttpGet("daily")]
    public async Task<IActionResult> Daily([FromQuery,Required] DateTime? from,[FromQuery,Required] DateTime? to)
    {
        if(!from.HasValue || !to.HasValue || !Valid(from.Value,to.Value)) return BadRequest(new {message="Khoảng thời gian không hợp lệ, tối đa 366 ngày."});
        var points=await InRange(from.Value,to.Value).Where(o=>o.IsPaid && o.Status=="Đã thanh toán")
            .GroupBy(o=>(o.PaidAt ?? o.OrderTime).Date).Select(g=>new {date=g.Key,revenue=g.Sum(o=>(decimal)o.TotalAmount),orders=g.Count()}).OrderBy(x=>x.date).ToListAsync();
        return Ok(new {from,to,points});
    }
}
