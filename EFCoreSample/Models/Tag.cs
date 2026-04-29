using System.ComponentModel.DataAnnotations;

namespace EFCoreSample.Models;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "標籤名稱")]
    public string Name { get; set; } = string.Empty;

    // Navigation - many-to-many with Post
    public List<Post> Posts { get; set; } = new();
}
