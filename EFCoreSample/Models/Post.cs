using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.Models;

public enum PostStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2
}

public class Post
{
    public int Id { get; set; }

    [Required]
    [MaxLength(300)]
    [Display(Name = "標題")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "內容")]
    public string? Content { get; set; }

    [Display(Name = "摘要")]
    [MaxLength(500)]
    public string? Summary { get; set; }

    [Display(Name = "發佈狀態")]
    public PostStatus Status { get; set; } = PostStatus.Draft;

    [Display(Name = "瀏覽次數")]
    public int ViewCount { get; set; }

    [Display(Name = "發佈時間")]
    public DateTime? PublishedAt { get; set; }

    [Display(Name = "建立時間")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "更新時間")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "部落格")]
    public int BlogId { get; set; }
    public Blog Blog { get; set; } = null!;

    [Display(Name = "分類")]
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    // Many-to-many with Tag
    public List<Tag> Tags { get; set; } = new();

    // Owned entity
    public PostMetadata? Metadata { get; set; }
}
