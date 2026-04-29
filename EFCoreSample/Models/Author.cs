using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.Models;

public class Author
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "姓名")]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(200)]
    [Display(Name = "電子郵件")]
    public string? Email { get; set; }

    [Display(Name = "簡介")]
    public string? Bio { get; set; }

    [Display(Name = "建立時間")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<Blog> Blogs { get; set; } = new();
}
