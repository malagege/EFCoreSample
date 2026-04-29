using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "分類名稱")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "說明")]
    public string? Description { get; set; }

    // Navigation
    public List<Post> Posts { get; set; } = new();
}
