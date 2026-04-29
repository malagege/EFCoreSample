# 第10章：資料庫遷移 (Migrations)

## 建立第一個 Migration

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 新增 Migration

修改 Model 後：

```bash
dotnet ef migrations add AddPostMetadata
dotnet ef database update
```

## 查看 Migration

```bash
dotnet ef migrations list
```

## 回滾 Migration

```bash
# 回到上一個 migration
dotnet ef database update PreviousMigrationName

# 移除最後一個 migration
dotnet ef migrations remove
```

## 生成 SQL Script

```bash
dotnet ef migrations script
```

## Seed Data (OnModelCreating)

```csharp
modelBuilder.Entity<Author>().HasData(
    new Author { Id = 1, Name = "張小明", Email = "ming@example.com" }
);
```

## 程式碼中套用 Migration

```csharp
await context.Database.MigrateAsync();
```

## 程式範例
參考: EFCoreSample/Data/AppDbContext.cs, EFCoreSample/Data/DbInitializer.cs
