using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;

[AutoValidateAntiforgeryToken]
public class ProductController : Controller
{
    private readonly AppDbContext db;
    private readonly IWebHostEnvironment env;
    public ProductController(AppDbContext db, IWebHostEnvironment env) { this.db=db; this.env=env; }
    public IActionResult Index(int? categoryId, string? search) => RedirectToAction(User.IsInRole("Admin") ? "Index" : "Menu", User.IsInRole("Admin") ? "CatalogAdmin" : "Cafe");
    public IActionResult Menu(int? categoryId) => RedirectToAction("Menu","Cafe");
    public IActionResult Category(int id) => RedirectToAction("Menu","Cafe");
    public async Task<IActionResult> Details(int id)
    {
        var p=await db.Products.AsNoTracking().Include(p=>p.Category).FirstOrDefaultAsync(p=>p.ProductID==id);
        return p==null ? NotFound() : View(p);
    }
    private void Categories(int? selected=null) => ViewBag.CategoryID=new SelectList(db.Categories.OrderBy(c=>c.Name),"CategoryID","Name",selected);
    private async Task<bool> Validate(Product product,IFormFile? imageFile)
    {
        if(!await db.Categories.AnyAsync(c=>c.CategoryID==product.CategoryID)) ModelState.AddModelError("CategoryID","Chọn danh mục hợp lệ.");
        if(imageFile!=null && imageFile.Length>0) {
            var extensions=new[]{".jpg",".jpeg",".png",".webp",".avif"};
            if(imageFile.Length>5*1024*1024 || !extensions.Contains(Path.GetExtension(imageFile.FileName).ToLowerInvariant()))
                ModelState.AddModelError("ImageUrl","Ảnh phải là JPG/PNG/WebP/AVIF, tối đa 5 MB.");
        }
        return ModelState.IsValid;
    }
    private async Task<string?> SaveImage(IFormFile? file)
    {
        if(file==null || file.Length==0) return null;
        var name=Guid.NewGuid().ToString("N")+Path.GetExtension(file.FileName).ToLowerInvariant();
        var folder=Path.Combine(env.WebRootPath,"images");Directory.CreateDirectory(folder);
        using var stream=System.IO.File.Create(Path.Combine(folder,name));await file.CopyToAsync(stream);
        return "/images/"+name;
    }
    [Authorize(Roles="Admin")]
    public IActionResult Create(){Categories();return View(new Product());}
    [HttpPost,Authorize(Roles="Admin")]
    public async Task<IActionResult> Create([Bind("Name,Price,CategoryID,Description,SupportsCustomization")] Product product,IFormFile? imageFile)
    {
        if(!await Validate(product,imageFile)){Categories(product.CategoryID);return View(product);}
        product.Name=product.Name.Trim();product.ImageUrl=await SaveImage(imageFile);product.IsAvailable=true;
        db.Products.Add(product);await db.SaveChangesAsync();return RedirectToAction("Index","CatalogAdmin");
    }
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        var p=await db.Products.FindAsync(id);if(p==null)return NotFound();Categories(p.CategoryID);return View(p);
    }
    [HttpPost,Authorize(Roles="Admin")]
    public async Task<IActionResult> Edit([Bind("ProductID,Name,Price,CategoryID,Description,SupportsCustomization")] Product product,IFormFile? imageFile)
    {
        var stored=await db.Products.FindAsync(product.ProductID);if(stored==null)return NotFound();
        if(!await Validate(product,imageFile)){product.ImageUrl=stored.ImageUrl;Categories(product.CategoryID);return View(product);}
        stored.Name=product.Name.Trim();stored.Price=product.Price;stored.CategoryID=product.CategoryID;stored.Description=product.Description;stored.SupportsCustomization=product.SupportsCustomization;
        stored.ImageUrl=await SaveImage(imageFile) ?? stored.ImageUrl;
        await db.SaveChangesAsync();return RedirectToAction("Index","CatalogAdmin");
    }
    [Authorize(Roles="Admin")]
    public IActionResult Delete(int? id)=>RedirectToAction("Index","CatalogAdmin");
    [HttpPost,ActionName("Delete"),Authorize(Roles="Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Soft availability change preserves historical foreign keys and receipts.
        var p=await db.Products.FindAsync(id);if(p==null)return NotFound();p.IsAvailable=false;
        await db.SaveChangesAsync();return RedirectToAction("Index","CatalogAdmin");
    }
}
