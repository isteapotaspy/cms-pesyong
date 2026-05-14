using CMS.Domain.Entities;
using CMS.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DeliveryTimeSlot)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ContactNameSnapshot)
             .HasMaxLength(150)
             .IsRequired();

        builder.Property(x => x.ContactEmailSnapshot)
            .HasMaxLength(150);

        builder.Property(x => x.ContactMobileSnapshot)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.CustomerNotes)
            .HasMaxLength(1000);

        builder.Property(x => x.SpecialInstructions)
            .HasMaxLength(1000);

        builder.Property(x => x.PromoCodeApplied)
            .HasMaxLength(50);

        builder.Property(x => x.SubTotal).HasPrecision(18, 2);
        builder.Property(x => x.DeliveryFee).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.GrandTotal).HasPrecision(18, 2);

        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();

        builder.HasOne(x => x.CustomerProfile)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.CustomerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Address)
            .WithMany()
            .HasForeignKey(x => x.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}