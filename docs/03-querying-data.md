# 第3章：查詢資料 (Querying Data)

## 基本查詢

```csharp
// 查詢所有
var blogs = await _db.Blogs.ToListAsync();

// 條件查詢
var highRated = await _db.Blogs.Where(b => b.Rating >= 4).ToListAsync();

// 查詢單一
var blog = await _db.Blogs.FirstOrDefaultAsync(b => b.Id == 1);
var blog2 = await _db.Blogs.FindAsync(1);
```

## 排序

```csharp
var sorted = await _db.Blogs
    .OrderByDescending(b => b.Rating)
    .ThenBy(b => b.Name)
    .ToListAsync();
```

## 分頁

```csharp
var page = 1; var pageSize = 10;
var paged = await _db.Blogs
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

## 投影 (Projection)

```csharp
var projected = await _db.Blogs
    .Select(b => new { b.Id, b.Name, b.Rating })
    .ToListAsync();
```

## 統計

```csharp
var count = await _db.Posts.CountAsync(p => p.Status == PostStatus.Published);
var total = await _db.Posts.SumAsync(p => p.ViewCount);
var max = await _db.Posts.MaxAsync(p => p.ViewCount);
var any = await _db.Posts.AnyAsync(p => p.Status == PostStatus.Published);
```

## GroupBy

```csharp
var grouped = await _db.Posts
    .GroupBy(p => p.Status)
    .Select(g => new { Status = g.Key, Count = g.Count() })
    .ToListAsync();
```

## 程式範例
參考: EFCoreSample/Controllers/EFDemoController.cs (BasicQuerying, AdvancedQuerying)
