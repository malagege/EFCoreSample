# 第8章：原始 SQL (Raw SQL Queries)

## FromSqlRaw

```csharp
var blogs = await _db.Blogs
    .FromSqlRaw("SELECT * FROM Blogs WHERE Rating >= 4")
    .ToListAsync();
```

## FromSqlInterpolated (安全的參數化查詢)

```csharp
int minRating = 3;
var blogs = await _db.Blogs
    .FromSqlInterpolated($"SELECT * FROM Blogs WHERE Rating >= {minRating}")
    .ToListAsync();
```

## ExecuteSqlRaw (非查詢操作)

```csharp
var affected = await _db.Database.ExecuteSqlRawAsync(
    "UPDATE Blogs SET Rating = Rating WHERE Id > 0");
```

## 注意事項

- 優先使用 `FromSqlInterpolated` 避免 SQL Injection
- `FromSqlRaw` 需要手動處理參數化
- 可以在 FromSql 後繼續串接 LINQ (混合查詢)

## 程式範例
參考: EFCoreSample/Controllers/EFDemoController.cs (RawSql)
