using CMS.Domain.Entities.Packages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Infrastructure.Persistence.Configurations;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.ToTable("Packages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.CardSummary)
            .HasMaxLength(500);

        builder.Property(x => x.Badge)
            .HasMaxLength(100);

        builder.Property(x => x.Notice)
            .HasMaxLength(150);

        builder.Property(x => x.ServesLabel)
            .HasMaxLength(100);

        builder.Property(x => x.InclusionText)
            .HasMaxLength(300);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Rating)
            .HasPrecision(4, 2);

        builder.HasOne(x => x.MenuCategory)
            .WithMany(x => x.Packages)
            .HasForeignKey(x => x.MenuCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}