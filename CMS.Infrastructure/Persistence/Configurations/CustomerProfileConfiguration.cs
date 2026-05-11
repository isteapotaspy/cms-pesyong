using CMS.Domain.Entities;
using CMS.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMS.Infrastructure.Persistence.Configurations;

public class CustomerProfileConfiguration : IEntityTypeConfiguration<CustomerProfile>
{
    public void Configure(EntityTypeBuilder<CustomerProfile> builder)
    {
        builder.ToTable("CustomerProfiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MobileNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasMany(x => x.Addresses)
            .WithOne(x => x.CustomerProfile)
            .HasForeignKey(x => x.CustomerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Orders)
            .WithOne(x => x.CustomerProfile)
            .HasForeignKey(x => x.CustomerProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}