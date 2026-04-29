using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.Models;

public class Blog
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Display(Name = "部落格名稱")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "說明")]
    public string? Description { get; set; }

    [Display(Name = "網址")]
    public string? Url { get; set; }

    [Display(Name = "評分")]
    public int Rating { get; set; }

    [Display(Name = "建立時間")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "作者")]
    public int? AuthorId { get; set; }
    public Author? Author { get; set; }

    // Navigation
    public List<Post> Posts { get; set; } = new();
}
