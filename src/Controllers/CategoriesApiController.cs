using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
namespace QuanLiQuanCaffee.Controllers;
[ApiController, Route("api/categories"), ApiExplorerSettings(GroupName = "catalog")]
[Authorize(Roles = "Admin,Staff"), AutoValidateAntiforgeryToken]
public class CategoriesApiController : ControllerBase
{
    private readonly AppDbContext db;
    public CategoriesApiController(AppDbContext db) { this.db = db; }
    public class Input { [Required, StringLength(100)] public string Name { get; set; } = ""; }
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await db.Categories.AsNoTracking()
        .OrderBy(c => c.Name).Select(c => new { c.CategoryID, c.Name }).ToListAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var c = await db.Categories.AsNoTracking().Where(c => c.CategoryID == id)
            .Select(c => new { c.CategoryID, c.Name }).FirstOrDefaultAsync();
        return c == null ? NotFound() : Ok(c);
    }
    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Input input)
    {
        if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new { message="Tên không được trống." });
        var c = new Category { Name=input.Name.Trim() }; db.Categories.Add(c);
        await db.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id=c.CategoryID }, new { c.CategoryID, c.Name });
    }
    [HttpPut("{id:int}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, Input input)
    {
        if (string.IsNullOrWhiteSpace(input.Name)) return BadRequest(new { message="Tên không được trống." });
        var c=await db.Categories.FindAsync(id); if(c==null) return NotFound();
        c.Name=input.Name.Trim(); await db.SaveChangesAsync(); return Ok(new { c.CategoryID, c.Name });
    }
    [HttpDelete("{id:int}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        using var tx=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var c=await db.Categories.FindAsync(id); if(c==null) return NotFound();
        if(await db.Products.AnyAsync(p=>p.CategoryID==id)) return Conflict(new {message="Danh mục còn món, không thể xóa."});
        db.Categories.Remove(c); await db.SaveChangesAsync(); await tx.CommitAsync(); return NoContent();
    }
}
