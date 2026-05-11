using CMS.Domain.Entities;
using CMS.Domain.Entities.Packages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Infrastructure.Persistence.Configurations;

public class PackageAddonConfiguration : IEntityTypeConfiguration<PackageAddon>
{
    public void Configure(EntityTypeBuilder<PackageAddon> builder)
    {
        builder.ToTable("PackageAddons");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Package)
            .WithMany(x => x.Addons)
            .HasForeignKey(x => x.PackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}