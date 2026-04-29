using EFCoreSample.Data;
using EFCoreSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample.Controllers;

/// <summary>
/// EFCore 功能展示控制器 - 涵蓋各種 EFCore 特性
/// </summary>
public class EFDemoController : Controller
{
    private readonly AppDbContext _db;

    public EFDemoController(AppDbContext db)
    {
        _db = db;
    }

    // GET /EFDemo
    public IActionResult Index()
    {
        return View();
    }

    // ===== 1. 基本查詢 (Basic Querying) =====
    public async Task<IActionResult> BasicQuerying()
    {
        // 1. 查詢所有
        var allBlogs = await _db.Blogs.ToListAsync();

        // 2. 條件查詢
        var highRatedBlogs = await _db.Blogs
            .Where(b => b.Rating >= 4)
            .ToListAsync();

        // 3. 單一查詢
        var firstBlog = await _db.Blogs.FirstOrDefaultAsync();
        var blogById = await _db.Blogs.FindAsync(1);

        // 4. 排序
        var sorted = await _db.Blogs
            .OrderByDescending(b => b.Rating)
            .ThenBy(b => b.Name)
            .ToListAsync();

        // 5. 分頁
        int page = 1, pageSize = 2;
        var paged = await _db.Blogs
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // 6. 投影 (Select / Anonymous type)
        var projected = await _db.Blogs
            .Select(b => new { b.Id, b.Name, b.Rating })
            .ToListAsync();

        ViewBag.AllBlogs = allBlogs;
        ViewBag.HighRated = highRatedBlogs;
        ViewBag.FirstBlog = firstBlog;
        ViewBag.Sorted = sorted;
        ViewBag.Paged = paged;
        ViewBag.Projected = projected;

        return View();
    }

    // ===== 2. 關聯查詢 (Loading Related Data) =====
    public async Task<IActionResult> RelatedData()
    {
        // 1. Eager Loading (Include)
        var blogsWithPosts = await _db.Blogs
            .Include(b => b.Posts)
            .Include(b => b.Author)
            .ToListAsync();

        // 2. ThenInclude
        var blogsDeep = await _db.Blogs
            .Include(b => b.Posts)
                .ThenInclude(p => p.Tags)
            .ToListAsync();

        // 3. Explicit Loading
        var blog = await _db.Blogs.FirstOrDefaultAsync();
        if (blog != null)
        {
            await _db.Entry(blog).Collection(b => b.Posts).LoadAsync();
            await _db.Entry(blog).Reference(b => b.Author).LoadAsync();
        }

        // 4. Select projection (avoid N+1)
        var blogSummaries = await _db.Blogs
            .Select(b => new
            {
                b.Name,
                PostCount = b.Posts.Count,
                AuthorName = b.Author != null ? b.Author.Name : "未知"
            })
            .ToListAsync();

        ViewBag.BlogsWithPosts = blogsWithPosts;
        ViewBag.BlogsDeep = blogsDeep;
        ViewBag.BlogSummaries = blogSummaries;
        ViewBag.ExplicitBlog = blog;

        return View();
    }

    // ===== 3. 資料儲存 (Saving Data) =====
    public async Task<IActionResult> SavingData()
    {
        var results = new List<string>();

        // 1. Add (Insert)
        var newAuthor = new Author
        {
            Name = $"示範作者_{DateTime.Now.Ticks}",
            Email = "demo@example.com",
            Bio = "示範用作者",
            CreatedAt = DateTime.UtcNow
        };
        _db.Authors.Add(newAuthor);
        await _db.SaveChangesAsync();
        results.Add($"新增作者 ID={newAuthor.Id}");

        // 2. Update
        newAuthor.Bio = "已更新的簡介";
        _db.Authors.Update(newAuthor);
        await _db.SaveChangesAsync();
        results.Add($"更新作者 ID={newAuthor.Id}");

        // 3. Delete
        _db.Authors.Remove(newAuthor);
        await _db.SaveChangesAsync();
        results.Add($"刪除作者成功");

        // 4. AddRange
        var tags = new[]
        {
            new Tag { Name = $"TempTag1_{DateTime.Now.Ticks}" },
            new Tag { Name = $"TempTag2_{DateTime.Now.Ticks}" }
        };
        _db.Tags.AddRange(tags);
        await _db.SaveChangesAsync();
        results.Add($"批次新增 {tags.Length} 個標籤");

        // Clean up temp tags
        _db.Tags.RemoveRange(tags);
        await _db.SaveChangesAsync();
        results.Add("清除暫時標籤");

        ViewBag.Results = results;
        return View();
    }

    // ===== 4. 交易 (Transactions) =====
    public async Task<IActionResult> Transactions()
    {
        var results = new List<string>();

        // 1. 隱式交易 (單次 SaveChanges 是交易)
        var blog1 = new Blog { Name = $"交易測試_{DateTime.Now.Ticks}", Rating = 3, CreatedAt = DateTime.UtcNow };
        _db.Blogs.Add(blog1);
        await _db.SaveChangesAsync();
        results.Add($"隱式交易: 新增部落格 ID={blog1.Id}");

        // 2. 明確交易
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var post1 = new Post
            {
                Title = $"交易測試文章_{DateTime.Now.Ticks}",
                Status = PostStatus.Draft,
                BlogId = blog1.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.Posts.Add(post1);
            await _db.SaveChangesAsync();

            blog1.Rating = 5;
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
            results.Add($"明確交易: 提交成功，文章 ID={post1.Id}");

            // Cleanup
            _db.Posts.Remove(post1);
            _db.Blogs.Remove(blog1);
            await _db.SaveChangesAsync();
            results.Add("清除測試資料");
        }
        catch
        {
            await transaction.RollbackAsync();
            results.Add("交易已回滾");
        }

        ViewBag.Results = results;
        return View();
    }

    // ===== 5. 原始 SQL (Raw SQL) =====
    public async Task<IActionResult> RawSql()
    {
        // 1. FromSqlRaw
        var blogs = await _db.Blogs
            .FromSqlRaw("SELECT * FROM Blogs WHERE Rating >= 4")
            .ToListAsync();

        // 2. ExecuteSqlRaw
        var affected = await _db.Database.ExecuteSqlRawAsync(
            "UPDATE Blogs SET Rating = Rating WHERE Id > 0");

        // 3. Interpolated SQL (safe)
        int minRating = 3;
        var filtered = await _db.Blogs
            .FromSqlInterpolated($"SELECT * FROM Blogs WHERE Rating >= {minRating}")
            .ToListAsync();

        ViewBag.Blogs = blogs;
        ViewBag.Affected = affected;
        ViewBag.Filtered = filtered;
        return View();
    }

    // ===== 6. 變更追蹤 (Change Tracking) =====
    public async Task<IActionResult> ChangeTracking()
    {
        var results = new List<string>();

        // 1. 追蹤實體狀態
        var blog = await _db.Blogs.FirstOrDefaultAsync();
        if (blog != null)
        {
            var entry = _db.Entry(blog);
            results.Add($"初始狀態: {entry.State}");

            blog.Rating = blog.Rating == 5 ? 4 : 5;
            results.Add($"修改後狀態: {entry.State}");

            // 顯示已變更的屬性
            foreach (var prop in entry.Properties.Where(p => p.IsModified))
            {
                results.Add($"  已修改欄位: {prop.Metadata.Name}, 原值: {prop.OriginalValue}, 新值: {prop.CurrentValue}");
            }

            // 不儲存，還原
            entry.State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
            results.Add($"還原後狀態: {entry.State}");
        }

        // 2. AsNoTracking (唯讀查詢，效能較好)
        var noTrackingBlogs = await _db.Blogs
            .AsNoTracking()
            .ToListAsync();
        results.Add($"AsNoTracking 查詢: {noTrackingBlogs.Count} 筆部落格 (不追蹤)");

        // 3. 查看所有追蹤的實體
        var trackedEntities = _db.ChangeTracker.Entries()
            .Select(e => $"{e.Entity.GetType().Name}: {e.State}")
            .ToList();
        results.Add($"目前追蹤中的實體: {string.Join(", ", trackedEntities)}");

        ViewBag.Results = results;
        return View();
    }

    // ===== 7. 進階查詢 (Advanced Querying) =====
    public async Task<IActionResult> AdvancedQuerying()
    {
        // 1. GroupBy
        var postsByStatus = await _db.Posts
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        // 2. Join
        var blogPostJoin = await _db.Blogs
            .Join(_db.Posts,
                b => b.Id,
                p => p.BlogId,
                (b, p) => new { BlogName = b.Name, PostTitle = p.Title })
            .ToListAsync();

        // 3. Any / All / Count
        var hasPublished = await _db.Posts.AnyAsync(p => p.Status == PostStatus.Published);
        var publishedCount = await _db.Posts.CountAsync(p => p.Status == PostStatus.Published);

        // 4. Sum / Max / Min / Average
        var totalViews = await _db.Posts.SumAsync(p => p.ViewCount);
        var maxViews = await _db.Posts.MaxAsync(p => p.ViewCount);

        // 5. Contains (IN clause)
        var ids = new[] { 1, 2 };
        var specificBlogs = await _db.Blogs
            .Where(b => ids.Contains(b.Id))
            .ToListAsync();

        ViewBag.PostsByStatus = postsByStatus;
        ViewBag.BlogPostJoin = blogPostJoin;
        ViewBag.HasPublished = hasPublished;
        ViewBag.PublishedCount = publishedCount;
        ViewBag.TotalViews = totalViews;
        ViewBag.MaxViews = maxViews;
        ViewBag.SpecificBlogs = specificBlogs;

        return View();
    }

    // ===== 8. 編譯查詢 (Compiled Queries) =====
    private static readonly Func<AppDbContext, int, Task<Blog?>> GetBlogByIdCompiled =
        EF.CompileAsyncQuery((AppDbContext ctx, int id) =>
            ctx.Blogs.FirstOrDefault(b => b.Id == id));

    public async Task<IActionResult> CompiledQueries()
    {
        var results = new List<string>();

        // 使用編譯查詢
        var blog = await GetBlogByIdCompiled(_db, 1);
        results.Add($"編譯查詢結果: {(blog != null ? blog.Name : "未找到")}");

        ViewBag.Results = results;
        ViewBag.Blog = blog;
        return View();
    }

    // ===== 9. 值轉換 (Value Conversions) =====
    public async Task<IActionResult> ValueConversions()
    {
        // PostStatus 被存為字串 (在 OnModelCreating 設定)
        var posts = await _db.Posts
            .Select(p => new { p.Title, p.Status })
            .ToListAsync();

        ViewBag.Posts = posts;
        return View();
    }

    // ===== 10. 擁有實體 (Owned Entities) =====
    public async Task<IActionResult> OwnedEntities()
    {
        // PostMetadata 是 Post 的擁有實體
        var post = await _db.Posts
            .FirstOrDefaultAsync(p => p.Id == 1);

        if (post != null && post.Metadata == null)
        {
            post.Metadata = new PostMetadata
            {
                SeoTitle = "EFCore 入門 - SEO Title",
                SeoDescription = "學習 EFCore 的最佳入門指南",
                FeaturedImageUrl = "https://example.com/image.jpg"
            };
            await _db.SaveChangesAsync();
        }

        ViewBag.Post = post;
        return View();
    }
}
