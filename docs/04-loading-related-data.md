# 第4章：載入關聯資料 (Loading Related Data)

## Eager Loading (Include)

```csharp
var blogs = await _db.Blogs
    .Include(b => b.Posts)
    .Include(b => b.Author)
    .ToListAsync();
```

## ThenInclude (巢狀載入)

```csharp
var blogs = await _db.Blogs
    .Include(b => b.Posts)
        .ThenInclude(p => p.Tags)
    .ToListAsync();
```

## Explicit Loading (顯式載入)

```csharp
var blog = await _db.Blogs.FirstAsync();
await _db.Entry(blog).Collection(b => b.Posts).LoadAsync();
await _db.Entry(blog).Reference(b => b.Author).LoadAsync();
```

## 投影查詢 (避免 N+1 問題)

```csharp
var summaries = await _db.Blogs
    .Select(b => new
    {
        b.Name,
        PostCount = b.Posts.Count,
        AuthorName = b.Author != null ? b.Author.Name : "未知"
    })
    .ToListAsync();
```

## 程式範例
參考: EFCoreSample/Controllers/EFDemoController.cs (RelatedData)
