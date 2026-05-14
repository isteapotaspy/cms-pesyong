using CMS.Domain.Entities;
using CMS.Domain.Entities.Packages;
using CMS.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PackageTitleSnapshot)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.SizeLabelSnapshot)
            .HasMaxLength(100);

        builder.Property(x => x.BaseUnitPrice)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Package)
            .WithMany()
            .HasForeignKey(x => x.PackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PackageSize)
            .WithMany()
            .HasForeignKey(x => x.PackageSizeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.MealSelections)
            .WithOne(x => x.OrderItem)
            .HasForeignKey(x => x.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.AddonSelections)
            .WithOne(x => x.OrderItem)
            .HasForeignKey(x => x.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}