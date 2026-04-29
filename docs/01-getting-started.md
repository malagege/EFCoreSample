# 第1章：EFCore 入門 (Getting Started)

## 安裝套件

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

## 建立 Model

```csharp
public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Post> Posts { get; set; } = new();
}
```

## 建立 DbContext

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Blog> Blogs { get; set; }
}
```

## 在 Program.cs 注入

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));
```

## 建立 Migration & 更新資料庫

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 程式範例
參考: EFCoreSample/Controllers/HomeController.cs
