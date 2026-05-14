using CMS.Domain.Entities;
using CMS.Domain.Entities.Packages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Infrastructure.Persistence.Configurations;

public class PackageSizeConfiguration : IEntityTypeConfiguration<PackageSize>
{
    public void Configure(EntityTypeBuilder<PackageSize> builder)
    {
        builder.ToTable("PackageSizes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Label)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Subtitle)
            .HasMaxLength(150);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Package)
            .WithMany(x => x.Sizes)
            .HasForeignKey(x => x.PackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}