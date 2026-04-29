# 第6章：變更追蹤 (Change Tracking)

## EntityState

```csharp
var blog = await _db.Blogs.FirstAsync();
var entry = _db.Entry(blog);
Console.WriteLine(entry.State); // Unchanged

blog.Rating = 5;
Console.WriteLine(entry.State); // Modified
```

## 查看已修改的屬性

```csharp
foreach (var prop in entry.Properties.Where(p => p.IsModified))
{
    Console.WriteLine($"{prop.Metadata.Name}: {prop.OriginalValue} → {prop.CurrentValue}");
}
```

## 還原變更

```csharp
entry.State = EntityState.Unchanged;
```

## AsNoTracking (提升唯讀效能)

```csharp
var blogs = await _db.Blogs
    .AsNoTracking()
    .ToListAsync();
```

## 查看所有追蹤實體

```csharp
var tracked = _db.ChangeTracker.Entries()
    .Select(e => $"{e.Entity.GetType().Name}: {e.State}")
    .ToList();
```

## 程式範例
參考: EFCoreSample/Controllers/EFDemoController.cs (ChangeTracking)
