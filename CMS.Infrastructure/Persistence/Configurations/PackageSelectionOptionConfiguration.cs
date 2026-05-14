using CMS.Domain.Entities;
using CMS.Domain.Entities.Packages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Infrastructure.Persistence.Configurations;

public class PackageSelectionOptionConfiguration : IEntityTypeConfiguration<PackageSelectionOption>
{
    public void Configure(EntityTypeBuilder<PackageSelectionOption> builder)
    {
        builder.ToTable("PackageSelectionOptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AdditionalPrice)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.PackageSelectionRule)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.PackageSelectionRuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Meal)
            .WithMany(x => x.PackageSelectionOptions)
            .HasForeignKey(x => x.MealId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}