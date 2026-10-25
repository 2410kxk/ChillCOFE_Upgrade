using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
namespace QuanLiQuanCaffee.Controllers;
[ApiController, Route("api/tables"), ApiExplorerSettings(GroupName="tables")]
[Authorize(Roles="Admin,Staff"), AutoValidateAntiforgeryToken]
public class TablesApiController : ControllerBase
{
    private readonly AppDbContext db;
    public TablesApiController(AppDbContext db) { this.db=db; }
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await db.Tables.AsNoTracking().OrderBy(t=>t.TableID)
        .Select(t=>new {t.TableID,t.Name,t.Status,t.Note}).ToListAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var t=await db.Tables.AsNoTracking().FirstOrDefaultAsync(t=>t.TableID==id);
        return t==null ? NotFound() : Ok(new {t.TableID,t.Name,t.Status,t.Note});
    }
    public class StatusInput { [Required] public string Status {get;set;}="Trống"; }
    [HttpPut("{id:int}/status"), Authorize(Roles="Admin")]
    public async Task<IActionResult> Status(int id, StatusInput input)
    {
        if(input.Status!="Trống" && input.Status!="Đang dùng") return BadRequest(new {message="Chỉ nhận Trống hoặc Đang dùng."});
        using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var table=await db.Tables.FindAsync(id); if(table==null) return NotFound();
        var busy=await db.Orders.AnyAsync(o=>o.TableID==id && !o.IsPaid && o.Status=="Đang xử lý");
        var expected=busy ? "Đang dùng" : "Trống";
        if(input.Status!=expected) return Conflict(new {message="Trạng thái phải khớp đơn đang mở. Không thể giải phóng bàn còn đơn.",expected});
        table.Status=expected; await db.SaveChangesAsync(); await tx.CommitAsync(); return Ok(new {table.TableID,table.Status});
    }
}
