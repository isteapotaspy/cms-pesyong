using CMS.Domain.Entities.Packages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Infrastructure.Persistence.Configurations;

public class PackageSelectionRuleConfiguration : IEntityTypeConfiguration<PackageSelectionRule>
{
    public void Configure(EntityTypeBuilder<PackageSelectionRule> builder)
    {
        builder.ToTable("PackageSelectionRules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasOne(x => x.PackageSize)
            .WithMany(x => x.SelectionRules)
            .HasForeignKey(x => x.PackageSizeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}