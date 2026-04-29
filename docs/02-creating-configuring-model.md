# 第2章：建立與設定 Model (Creating and Configuring a Model)

## Data Annotations

```csharp
[Required]
[MaxLength(200)]
public string Name { get; set; } = string.Empty;

[EmailAddress]
public string? Email { get; set; }
```

## Fluent API (OnModelCreating)

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Blog>(entity =>
    {
        entity.HasKey(b => b.Id);
        entity.Property(b => b.Name).IsRequired().HasMaxLength(200);
        entity.Property(b => b.Rating).HasDefaultValue(0);
    });
}
```

## 值轉換 (Value Conversion) - Enum to String

```csharp
entity.Property(p => p.Status)
      .HasConversion<string>()
      .HasMaxLength(20);
```

## 擁有實體 (Owned Entity)

```csharp
entity.OwnsOne(p => p.Metadata, meta =>
{
    meta.Property(m => m.SeoTitle).HasMaxLength(200);
});
```

## 索引

```csharp
entity.HasIndex(p => p.Status);
entity.HasIndex(p => p.PublishedAt);
```

## 程式範例
參考: EFCoreSample/Data/AppDbContext.cs
