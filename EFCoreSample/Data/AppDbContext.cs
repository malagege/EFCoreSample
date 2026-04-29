using EFCoreSample.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API: Blog configuration
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Name).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Rating).HasDefaultValue(0);

            // One-to-many: Author -> Blog
            entity.HasOne(b => b.Author)
                  .WithMany(a => a.Blogs)
                  .HasForeignKey(b => b.AuthorId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Fluent API: Post configuration
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Title).IsRequired().HasMaxLength(300);
            entity.Property(p => p.Status)
                  .HasConversion<string>()
                  .HasMaxLength(20);

            // One-to-many: Blog -> Post
            entity.HasOne(p => p.Blog)
                  .WithMany(b => b.Posts)
                  .HasForeignKey(p => p.BlogId)
                  .OnDelete(DeleteBehavior.Cascade);

            // One-to-many: Category -> Post
            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Posts)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Many-to-many: Post <-> Tag
            entity.HasMany(p => p.Tags)
                  .WithMany(t => t.Posts)
                  .UsingEntity("PostTag");

            // Owned entity
            entity.OwnsOne(p => p.Metadata, meta =>
            {
                meta.Property(m => m.SeoTitle).HasMaxLength(200);
                meta.Property(m => m.SeoDescription).HasMaxLength(500);
            });

            // Index
            entity.HasIndex(p => p.Status);
            entity.HasIndex(p => p.PublishedAt);
        });

        // Seed data
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, Name = "張小明", Email = "ming@example.com", Bio = "資深後端工程師", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Author { Id = 2, Name = "李美華", Email = "hua@example.com", Bio = "前端技術愛好者", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "技術", Description = "技術相關文章" },
            new Category { Id = 2, Name = "生活", Description = "生活點滴" },
            new Category { Id = 3, Name = "教學", Description = "教學文章" }
        );

        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = 1, Name = "EFCore" },
            new Tag { Id = 2, Name = ".NET" },
            new Tag { Id = 3, Name = "C#" },
            new Tag { Id = 4, Name = "資料庫" }
        );

        modelBuilder.Entity<Blog>().HasData(
            new Blog { Id = 1, Name = "EFCore 學習筆記", Description = "記錄 EFCore 學習過程", Rating = 5, AuthorId = 1, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Blog { Id = 2, Name = "技術分享", Description = ".NET 技術文章", Rating = 4, AuthorId = 2, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<Post>().HasData(
            new Post { Id = 1, Title = "EFCore 入門教學", Content = "這是 EFCore 入門文章...", Summary = "快速了解 EFCore 基礎", Status = PostStatus.Published, ViewCount = 100, BlogId = 1, CategoryId = 3, PublishedAt = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Post { Id = 2, Title = "EFCore 查詢技巧", Content = "進階查詢方式...", Summary = "各種查詢技巧介紹", Status = PostStatus.Published, ViewCount = 80, BlogId = 1, CategoryId = 1, PublishedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Post { Id = 3, Title = "草稿文章", Content = "待完成...", Summary = "尚未發佈", Status = PostStatus.Draft, ViewCount = 0, BlogId = 2, CategoryId = null, PublishedAt = null, CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
