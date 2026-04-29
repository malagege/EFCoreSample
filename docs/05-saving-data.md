# 第5章：儲存資料 (Saving Data)

## Add (新增)

```csharp
var author = new Author { Name = "張三", Email = "san@example.com" };
_db.Authors.Add(author);
await _db.SaveChangesAsync();
// author.Id 已被填入
```

## Update (更新)

```csharp
author.Bio = "更新的簡介";
_db.Authors.Update(author);
await _db.SaveChangesAsync();
```

## Delete (刪除)

```csharp
_db.Authors.Remove(author);
await _db.SaveChangesAsync();
```

## 批次操作

```csharp
_db.Tags.AddRange(tag1, tag2, tag3);
_db.Tags.RemoveRange(tags);
await _db.SaveChangesAsync();
```

## 程式範例
參考: EFCoreSample/Controllers/EFDemoController.cs (SavingData)
