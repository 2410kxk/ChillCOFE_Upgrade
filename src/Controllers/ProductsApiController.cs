using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
namespace QuanLiQuanCaffee.Controllers;
[ApiController, Route("api/products"), ApiExplorerSettings(GroupName = "catalog")]
[Authorize(Roles = "Admin,Staff"), AutoValidateAntiforgeryToken]
public class ProductsApiController : ControllerBase
{
    private readonly AppDbContext db;
    public ProductsApiController(AppDbContext db) { this.db=db; }
    public class Input
    {
        [Required, StringLength(150)] public string Name { get; set; } = "";
        [Range(0,10000000)] public decimal Price { get; set; }
        [Range(1,int.MaxValue)] public int CategoryId { get; set; }
        [StringLength(2000)] public string? Description { get; set; }
        [StringLength(500)] public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool SupportsCustomization { get; set; } = true;
    }
    [HttpGet]
    public async Task<IActionResult> List(string? search=null, int? categoryId=null, bool? available=null,
        [Range(1,int.MaxValue)] int page=1, [Range(1,100)] int pageSize=20)
    {
        var q=db.Products.AsNoTracking().AsQueryable();
        if(!string.IsNullOrWhiteSpace(search)) q=q.Where(p=>p.Name.Contains(search.Trim()));
        if(categoryId.HasValue) q=q.Where(p=>p.CategoryID==categoryId.Value);
        if(available.HasValue) q=q.Where(p=>p.IsAvailable==available.Value);
        var offset=((long)page-1)*pageSize;
        if(offset>int.MaxValue) return BadRequest(new {message="Trang vượt giới hạn."});
        var count=await q.CountAsync();
        var items=await q.OrderBy(p=>p.ProductID).Skip((int)offset).Take(pageSize)
            .Select(p=>new {p.ProductID,p.Name,p.Price,p.CategoryID,p.Description,p.ImageUrl,p.IsAvailable,p.SupportsCustomization}).ToListAsync();
        return Ok(new {page,pageSize,count,items});
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var p=await db.Products.AsNoTracking().Where(p=>p.ProductID==id)
            .Select(p=>new {p.ProductID,p.Name,p.Price,p.CategoryID,p.Description,p.ImageUrl,p.IsAvailable,p.SupportsCustomization}).FirstOrDefaultAsync();
        return p==null ? NotFound() : Ok(p);
    }
    private async Task<bool> Valid(Input input) => !string.IsNullOrWhiteSpace(input.Name)
        && await db.Categories.AnyAsync(c=>c.CategoryID==input.CategoryId)
        && (string.IsNullOrWhiteSpace(input.ImageUrl) ||
            (input.ImageUrl.StartsWith("/images/",StringComparison.Ordinal) && !input.ImageUrl.Contains("..")));
    private static void Apply(Product p, Input i)
    {
        p.Name=i.Name.Trim(); p.Price=(double)i.Price; p.CategoryID=i.CategoryId;
        p.Description=i.Description; p.ImageUrl=i.ImageUrl; p.IsAvailable=i.IsAvailable;
        p.SupportsCustomization=i.SupportsCustomization;
    }
    [HttpPost, Authorize(Roles="Admin")]
    public async Task<IActionResult> Create(Input input)
    {
        if(!await Valid(input)) return BadRequest(new {message="Kiểm tra tên, danh mục; ảnh dùng đường dẫn /images/... hoặc để trống."});
        var p=new Product(); Apply(p,input); db.Products.Add(p); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get),new {id=p.ProductID},new {p.ProductID,p.Name,p.Price,p.IsAvailable});
    }
    [HttpPut("{id:int}"), Authorize(Roles="Admin")]
    public async Task<IActionResult> Update(int id, Input input)
    {
        var p=await db.Products.FindAsync(id); if(p==null) return NotFound();
        if(!await Valid(input)) return BadRequest(new {message="Tên, danh mục hoặc đường dẫn ảnh không hợp lệ."});
        Apply(p,input); await db.SaveChangesAsync(); return Ok(new {p.ProductID,p.Name,p.Price,p.IsAvailable});
    }
    [HttpDelete("{id:int}"), Authorize(Roles="Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var p=await db.Products.FindAsync(id); if(p==null) return NotFound();
        p.IsAvailable=false; await db.SaveChangesAsync();
        return Ok(new {message="Đã ngừng bán; giữ món để bảo toàn lịch sử đơn.",p.ProductID,p.IsAvailable});
    }
}
