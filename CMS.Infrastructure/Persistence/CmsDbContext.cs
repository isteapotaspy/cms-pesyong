using CMS.Domain.Entities;
using CMS.Domain.Entities.Menu;
using CMS.Domain.Entities.Orders;
using CMS.Domain.Entities.Packages;
using CMS.Domain.Entities.Payment;
using CMS.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace CMS.Infrastructure.Persistence;

public class CmsDbContext : DbContext
{
    public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options)
    {
    }

    // Users
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();
    public DbSet<Address> Addresses => Set<Address>();

    // Menu
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<Meal> Meals => Set<Meal>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PackageSize> PackageSizes => Set<PackageSize>();
    public DbSet<PackageAddon> PackageAddons => Set<PackageAddon>();
    public DbSet<PackageSelectionRule> PackageSelectionRules => Set<PackageSelectionRule>();
    public DbSet<PackageSelectionOption> PackageSelectionOptions => Set<PackageSelectionOption>();

    // Orders
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderItemMealSelection> OrderItemMealSelections => Set<OrderItemMealSelection>();
    public DbSet<OrderItemAddonSelection> OrderItemAddonSelections => Set<OrderItemAddonSelection>();

    // Support
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<DeliveryUpdate> DeliveryUpdates => Set<DeliveryUpdate>();
    public DbSet<Promo> Promos => Set<Promo>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // This will auto-load all IEntityTypeConfiguration<T> classes later
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CmsDbContext).Assembly);
    }
}