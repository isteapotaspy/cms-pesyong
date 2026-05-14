using CMS.Domain.Entities;
using CMS.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Infrastructure.Persistence.Configurations;

public class OrderItemMealSelectionConfiguration : IEntityTypeConfiguration<OrderItemMealSelection>
{
    public void Configure(EntityTypeBuilder<OrderItemMealSelection> builder)
    {
        builder.ToTable("OrderItemMealSelections");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RuleTitleSnapshot)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.MealNameSnapshot)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.AdditionalPrice)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.PackageSelectionRule)
            .WithMany()
            .HasForeignKey(x => x.PackageSelectionRuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Meal)
            .WithMany()
            .HasForeignKey(x => x.MealId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}