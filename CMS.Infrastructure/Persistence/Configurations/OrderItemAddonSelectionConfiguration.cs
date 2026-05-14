using CMS.Domain.Entities;
using CMS.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Infrastructure.Persistence.Configurations;

public class OrderItemAddonSelectionConfiguration : IEntityTypeConfiguration<OrderItemAddonSelection>
{
    public void Configure(EntityTypeBuilder<OrderItemAddonSelection> builder)
    {
        builder.ToTable("OrderItemAddonSelections");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AddonNameSnapshot)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.AdditionalPrice)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.PackageAddon)
            .WithMany()
            .HasForeignKey(x => x.PackageAddonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}