# 第7章：交易 (Transactions)

## 隱式交易

每次 SaveChanges() 都包在一個交易中：

```csharp
_db.Blogs.Add(blog);
_db.Posts.Add(post);
await _db.SaveChangesAsync(); // 一個交易
```

## 明確交易

```csharp
using var transaction = await _db.Database.BeginTransactionAsync();
try
{
    _db.Posts.Add(post);
    await _db.SaveChangesAsync();

    blog.Rating = 5;
    await _db.SaveChangesAsync();

    await transaction.CommitAsync(); // 提交
}
catch
{
    await transaction.RollbackAsync(); // 回滾
}
```

## 程式範例
參考: EFCoreSample/Controllers/EFDemoController.cs (Transactions)
