# 第9章：關聯 (Relationships)

## 一對多 (One-to-Many)

```csharp
// Author → Blog
public class Author
{
    public int Id { get; set; }
    public List<Blog> Blogs { get; set; } = new();
}

public class Blog
{
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}

// Fluent API
entity.HasOne(b => b.Author)
      .WithMany(a => a.Blogs)
      .HasForeignKey(b => b.AuthorId)
      .OnDelete(DeleteBehavior.SetNull);
```

## 多對多 (Many-to-Many)

```csharp
// Post ↔ Tag
public class Post
{
    public List<Tag> Tags { get; set; } = new();
}

public class Tag
{
    public List<Post> Posts { get; set; } = new();
}

// Fluent API (EF Core 5+ 自動產生 Join Table)
entity.HasMany(p => p.Tags)
      .WithMany(t => t.Posts)
      .UsingEntity("PostTag");
```

## 擁有實體 (Owned Entity)

```csharp
public class Post
{
    public PostMetadata? Metadata { get; set; }
}

// PostMetadata 的欄位直接存在 Posts 資料表
entity.OwnsOne(p => p.Metadata);
```

## 程式範例
參考: EFCoreSample/Data/AppDbContext.cs
