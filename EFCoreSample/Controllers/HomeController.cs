using System.Diagnostics;
using EFCoreSample.Data;
using EFCoreSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _db;

    public HomeController(ILogger<HomeController> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var blogs = await _db.Blogs
            .Include(b => b.Author)
            .Include(b => b.Posts)
            .OrderByDescending(b => b.Rating)
            .ToListAsync();
        return View(blogs);
    }

    public async Task<IActionResult> Posts(int? blogId, int? categoryId, string? search, int page = 1)
    {
        const int pageSize = 5;

        var query = _db.Posts
            .Include(p => p.Blog)
            .Include(p => p.Category)
            .Include(p => p.Tags)
            .Where(p => p.Status == PostStatus.Published);

        if (blogId.HasValue)
            query = query.Where(p => p.BlogId == blogId.Value);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Title.Contains(search) || (p.Summary != null && p.Summary.Contains(search)));

        var total = await query.CountAsync();
        var posts = await query
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Total = total;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.BlogId = blogId;
        ViewBag.CategoryId = categoryId;
        ViewBag.Search = search;
        ViewBag.Categories = await _db.Categories.ToListAsync();
        ViewBag.Blogs = await _db.Blogs.ToListAsync();

        return View(posts);
    }

    public async Task<IActionResult> PostDetail(int id)
    {
        var post = await _db.Posts
            .Include(p => p.Blog).ThenInclude(b => b.Author)
            .Include(p => p.Category)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) return NotFound();

        // Increment view count (demonstrates SaveChanges)
        post.ViewCount++;
        post.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return View(post);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
