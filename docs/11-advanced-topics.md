# 第11章：進階主題 (Advanced Topics)

## 編譯查詢 (Compiled Queries)

```csharp
// 定義 (靜態欄位)
private static readonly Func<AppDbContext, int, Task<Blog?>> GetBlogById =
    EF.CompileAsyncQuery((AppDbContext ctx, int id) =>
        ctx.Blogs.FirstOrDefault(b => b.Id == id));

// 使用
var blog = await GetBlogById(_db, 1);
```

## 值轉換 (Value Conversions)

```csharp
// Enum 存為字串
entity.Property(p => p.Status)
      .HasConversion<string>()
      .HasMaxLength(20);

// 自訂轉換
entity.Property(p => p.Flags)
      .HasConversion(
          v => string.Join(",", v),
          v => v.Split(",", StringSplitOptions.None).ToList()
      );
```

## 擁有實體 (Owned Entities)

```csharp
entity.OwnsOne(p => p.Metadata, meta =>
{
    meta.Property(m => m.SeoTitle).HasMaxLength(200);
    meta.Property(m => m.SeoDescription).HasMaxLength(500);
});
// PostMetadata 的欄位嵌入在 Posts 資料表中
```

## 原始 SQL 混合 LINQ

```csharp
var blogs = await _db.Blogs
    .FromSqlRaw("SELECT * FROM Blogs")
    .Where(b => b.Rating >= 4)  // LINQ 繼續串接
    .OrderBy(b => b.Name)
    .ToListAsync();
```

## 全域查詢篩選器 (Global Query Filters)

```csharp
// 軟刪除範例
modelBuilder.Entity<Post>()
    .HasQueryFilter(p => !p.IsDeleted);
```

## 攔截器 (Interceptors)

```csharp
public class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        // 在儲存前記錄變更
        return base.SavingChanges(eventData, result);
    }
}
```

## 程式範例
參考: EFCoreSample/Controllers/EFDemoController.cs (CompiledQueries, ValueConversions, OwnedEntities)
