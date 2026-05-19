using CMS.Contracts.Customer.Menu;
using CMS.Contracts.Customer.Orders;
using CMS.Contracts.Customer.Meals;
using CMS.Contracts.Customer.Auth;
using CMS.Contracts.Customer.Profile;
using CMS.Domain.Entities;
using CMS.Domain.Entities.Orders;
using CMS.Domain.Entities.Packages;
using CMS.Domain.Entities.User;
using CMS.Domain.Entities.Menu;
using CMS.Domain.Enums;
using CMS.Contracts.Admin.Dashboard;
using CMS.Contracts.Admin.Package;
using CMS.Contracts.Admin.Orders;
using CMS.Contracts.Admin.Customers;
using CMS.Contracts.Admin.Meals;
using CMS.Infrastructure;
using CMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CMS.Server.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CMS.Server.Services;

namespace CMS.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                //options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PesyongWeb", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:7110",
                            "https://localhost:7110")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.Configure<SmtpOptions>(
            builder.Configuration.GetSection("Smtp"));

            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

            builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection("Jwt"));

            builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();

            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
            var jwtKey = Encoding.UTF8.GetBytes(jwtOptions.Key);

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();
            
            builder.Services.AddControllers();

            // Add Swagger/OpenAPI for visual API editing
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type =>
                {
                    return type.FullName.Replace("+", ".").Replace("[", "_").Replace("]", "_");
                });
            });


            var app = builder.Build();

            app.UseCors("PesyongWeb");

            app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                //app.MapOpenApi();
                await app.Services.SeedDatabaseAsync();
            }


            // =================== CUSTOMER ENDPOINTS =================== //
            //CHECK if the connection succeeds in ADMIN
            app.MapGet("/api/ping", () =>
            {
                return Results.Ok(new
                {
                    message = "CMS API is running",
                    time = DateTimeOffset.Now
                });
            });

            //GET all menu data for customer menu page
            app.MapGet("/api/customer/menu", async (CmsDbContext db) =>
            {
                var categories = await db.MenuCategories
                    .AsNoTracking()
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.Name)
                    .Select(x => new MenuCategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Slug = x.Slug
                    })
                    .ToListAsync();

                var packages = await db.Packages
                    .AsNoTracking()
                    .Where(x => x.IsAvailable)
                    .Include(x => x.MenuCategory)
                    .Include(x => x.Addons)
                    .Include(x => x.Sizes)
                        .ThenInclude(s => s.SelectionRules)
                            .ThenInclude(r => r.Options)
                                .ThenInclude(o => o.Meal)
                    .OrderBy(x => x.Title)
                    .Select(x => new MenuPackageDto
                    {
                        Id = x.Id,
                        CategoryId = x.MenuCategoryId,
                        CategoryName = x.MenuCategory.Name,

                        Title = x.Title,
                        Description = x.Description,
                        CardSummary = x.CardSummary,
                        Badge = x.Badge,
                        Notice = x.Notice,
                        ServesLabel = x.ServesLabel,
                        InclusionText = x.InclusionText,
                        ImageUrl = x.ImageUrl,

                        Rating = x.Rating,
                        ReviewCount = x.ReviewCount,

                        IsAvailable = x.IsAvailable,
                        IsCustomizable = x.IsCustomizable,

                        Sizes = x.Sizes
                            .Where(s => s.IsAvailable)
                            .OrderBy(s => s.PaxCount)
                            .Select(s => new PackageSizeDto
                            {
                                Id = s.Id,
                                Label = s.Label,
                                Subtitle = s.Subtitle,
                                PaxCount = s.PaxCount,
                                Price = s.Price,
                                SelectionRules = s.SelectionRules
                                    .Where(r => r.IsActive)
                                    .OrderBy(r => r.DisplayOrder)
                                    .Select(r => new PackageSelectionRuleDto
                                    {
                                        Id = r.Id,
                                        Title = r.Title,
                                        Description = r.Description,
                                        SelectionType = r.SelectionType.ToString(),
                                        AllowedMealType = r.AllowedMealType.ToString(),
                                        MinSelections = r.MinSelections,
                                        MaxSelections = r.MaxSelections,
                                        IsRequired = r.IsRequired,
                                        DisplayOrder = r.DisplayOrder,
                                        Options = r.Options
                                            .Where(o => o.IsActive && o.Meal.IsAvailable)
                                            .OrderBy(o => o.Meal.Name)
                                            .Select(o => new PackageSelectionOptionDto
                                            {
                                                Id = o.Id,
                                                MealId = o.MealId,
                                                AdditionalPrice = o.AdditionalPrice,
                                                IsDefault = o.IsDefault,
                                                Meal = new MealOptionDto
                                                {
                                                    Id = o.Meal.Id,
                                                    Name = o.Meal.Name,
                                                    Description = o.Meal.Description,
                                                    MealType = o.Meal.MealType.ToString(),
                                                    BasePrice = o.Meal.BasePrice,
                                                    AdditionalPrice = o.AdditionalPrice,
                                                    ImageUrl = o.Meal.ImageUrl,
                                                    IsDefault = o.IsDefault
                                                }
                                            })
                                            .ToList()
                                    })
                                    .ToList()
                            })
                            .ToList(),

                        Addons = x.Addons
                            .Where(a => a.IsAvailable)
                            .OrderBy(a => a.Name)
                            .Select(a => new AddonDto
                            {
                                Id = a.Id,
                                Name = a.Name,
                                Description = a.Description,
                                Price = a.Price,
                                IsAvailable = a.IsAvailable
                            })
                            .ToList()
                    })
                    .ToListAsync();

                var response = new GetMenuResponse
                {
                    Categories = categories,
                    Packages = packages
                };

                return Results.Ok(response);
            });


            //GET by id
            app.MapGet("/api/customer/packages/{id:int}", async (int id, CmsDbContext db) =>
            {
                var package = await db.Packages
                    .AsNoTracking()
                    .Where(x => x.Id == id && x.IsAvailable)
                    .Include(x => x.MenuCategory)
                    .Include(x => x.Addons)
                    .Include(x => x.Sizes)
                        .ThenInclude(s => s.SelectionRules)
                            .ThenInclude(r => r.Options)
                                .ThenInclude(o => o.Meal)
                    .Select(x => new MenuPackageDto
                    {
                        Id = x.Id,
                        CategoryId = x.MenuCategoryId,
                        CategoryName = x.MenuCategory.Name,

                        Title = x.Title,
                        Description = x.Description,
                        CardSummary = x.CardSummary,
                        Badge = x.Badge,
                        Notice = x.Notice,
                        ServesLabel = x.ServesLabel,
                        InclusionText = x.InclusionText,
                        ImageUrl = x.ImageUrl,

                        Rating = x.Rating,
                        ReviewCount = x.ReviewCount,

                        IsAvailable = x.IsAvailable,
                        IsCustomizable = x.IsCustomizable,

                        Sizes = x.Sizes
                            .Where(s => s.IsAvailable)
                            .OrderBy(s => s.PaxCount)
                            .Select(s => new PackageSizeDto
                            {
                                Id = s.Id,
                                Label = s.Label,
                                Subtitle = s.Subtitle,
                                PaxCount = s.PaxCount,
                                Price = s.Price,
                                SelectionRules = s.SelectionRules
                                    .Where(r => r.IsActive)
                                    .OrderBy(r => r.DisplayOrder)
                                    .Select(r => new PackageSelectionRuleDto
                                    {
                                        Id = r.Id,
                                        Title = r.Title,
                                        Description = r.Description,
                                        SelectionType = r.SelectionType.ToString(),
                                        AllowedMealType = r.AllowedMealType.ToString(),
                                        MinSelections = r.MinSelections,
                                        MaxSelections = r.MaxSelections,
                                        IsRequired = r.IsRequired,
                                        DisplayOrder = r.DisplayOrder,
                                        Options = r.Options
                                            .Where(o => o.IsActive && o.Meal.IsAvailable)
                                            .OrderBy(o => o.Meal.Name)
                                            .Select(o => new PackageSelectionOptionDto
                                            {
                                                Id = o.Id,
                                                MealId = o.MealId,
                                                AdditionalPrice = o.AdditionalPrice,
                                                IsDefault = o.IsDefault,
                                                Meal = new MealOptionDto
                                                {
                                                    Id = o.Meal.Id,
                                                    Name = o.Meal.Name,
                                                    Description = o.Meal.Description,
                                                    MealType = o.Meal.MealType.ToString(),
                                                    BasePrice = o.Meal.BasePrice,
                                                    AdditionalPrice = o.AdditionalPrice,
                                                    ImageUrl = o.Meal.ImageUrl,
                                                    IsDefault = o.IsDefault
                                                }
                                            })
                                            .ToList()
                                    })
                                    .ToList()
                            })
                            .ToList(),

                        Addons = x.Addons
                            .Where(a => a.IsAvailable)
                            .OrderBy(a => a.Name)
                            .Select(a => new AddonDto
                            {
                                Id = a.Id,
                                Name = a.Name,
                                Description = a.Description,
                                Price = a.Price,
                                IsAvailable = a.IsAvailable
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                return package is not null
                    ? Results.Ok(package)
                    : Results.NotFound(new { message = $"Package with id {id} was not found." });
            });


            //POST place order
            app.MapPost("/api/customer/orders", async (PlaceOrderRequest request, CmsDbContext db) =>
            {
                if (request.Items is null || request.Items.Count == 0)
                {
                    return Results.BadRequest(new { message = "At least one order item is required." });
                }

                if (!TryResolvePaymentMethod(request.PaymentMethod, out var paymentMethod))
                {
                    return Results.BadRequest(new { message = $"Unsupported payment method: '{request.PaymentMethod}'." });
                }

                CustomerProfile? customerProfile;

                if (request.CustomerProfileId.HasValue)
                {
                    customerProfile = await db.CustomerProfiles
                        .FirstOrDefaultAsync(x => x.Id == request.CustomerProfileId.Value);
                }
                else
                {
                    customerProfile = await db.CustomerProfiles
                        .OrderBy(x => x.Id)
                        .FirstOrDefaultAsync();
                }

                if (customerProfile is null)
                {
                    return Results.BadRequest(new { message = "Customer profile was not found." });
                }

                var address = new Address
                {
                    CustomerProfileId = customerProfile.Id,
                    StreetAddress = request.DeliveryAddress.StreetAddress,
                    Barangay = request.DeliveryAddress.Barangay,
                    City = request.DeliveryAddress.City,
                    Landmark = request.DeliveryAddress.Landmark,
                    Latitude = request.DeliveryAddress.Latitude,
                    Longitude = request.DeliveryAddress.Longitude,
                    IsDefault = false
                };

                var orderItems = new List<OrderItem>();
                decimal subTotal = 0m;

                foreach (var item in request.Items)
                {
                    if (item.Quantity <= 0)
                    {
                        return Results.BadRequest(new { message = "Order item quantity must be greater than zero." });
                    }

                    if (!TryResolveOrderItemType(item, out var itemType))
                    {
                        return Results.BadRequest(new
                        {
                            message = $"Unsupported item type '{item.ItemType}'. Allowed values are Package or Meal."
                        });
                    }

                    // =========================
                    // MEAL ITEM FLOW
                    // =========================
                    if (itemType == OrderItemType.Meal)
                    {
                        if (!item.MealId.HasValue)
                        {
                            return Results.BadRequest(new { message = "MealId is required for meal order items." });
                        }

                        if ((item.MealSelections?.Count ?? 0) > 0 || (item.AddonSelections?.Count ?? 0) > 0)
                        {
                            return Results.BadRequest(new
                            {
                                message = "Meal order items do not support package meal selections or add-ons."
                            });
                        }

                        var meal = await db.Meals
                            .FirstOrDefaultAsync(x => x.Id == item.MealId.Value && x.IsAvailable);

                        if (meal is null)
                        {
                            return Results.BadRequest(new
                            {
                                message = $"Meal with id {item.MealId.Value} was not found or is unavailable."
                            });
                        }

                        if (item.Quantity < meal.MinOrderQuantity)
                        {
                            return Results.BadRequest(new
                            {
                                message = $"Meal '{meal.Name}' requires a minimum order quantity of {meal.MinOrderQuantity}."
                            });
                        }

                        var mealOrderItem = new OrderItem
                        {
                            ItemType = OrderItemType.Meal,
                            MealId = meal.Id,
                            PackageId = null,
                            PackageSizeId = null,
                            PackageTitleSnapshot = meal.Name,
                            SizeLabelSnapshot = "Short Order",
                            BaseUnitPrice = meal.BasePrice,
                            Quantity = item.Quantity,
                            MealSelections = new List<OrderItemMealSelection>(),
                            AddonSelections = new List<OrderItemAddonSelection>()
                        };

                        subTotal += mealOrderItem.LineTotal;
                        orderItems.Add(mealOrderItem);

                        continue;
                    }

                    // =========================
                    // PACKAGE ITEM FLOW
                    // =========================
                    if (!item.PackageId.HasValue)
                    {
                        return Results.BadRequest(new { message = "PackageId is required for package order items." });
                    }

                    var package = await db.Packages
                        .Include(x => x.Addons)
                        .Include(x => x.Sizes)
                            .ThenInclude(s => s.SelectionRules)
                                .ThenInclude(r => r.Options)
                                    .ThenInclude(o => o.Meal)
                        .FirstOrDefaultAsync(x => x.Id == item.PackageId.Value && x.IsAvailable);

                    if (package is null)
                    {
                        return Results.BadRequest(new { message = $"Package with id {item.PackageId.Value} was not found or is unavailable." });
                    }

                    PackageSize? selectedSize = null;

                    if (item.PackageSizeId.HasValue)
                    {
                        selectedSize = package.Sizes.FirstOrDefault(x => x.Id == item.PackageSizeId.Value && x.IsAvailable);
                    }
                    else if (package.Sizes.Count == 1)
                    {
                        selectedSize = package.Sizes.FirstOrDefault(x => x.IsAvailable);
                    }

                    if (selectedSize is null)
                    {
                        return Results.BadRequest(new { message = $"A valid package size is required for package '{package.Title}'." });
                    }

                    var activeRules = selectedSize.SelectionRules
                        .Where(x => x.IsActive)
                        .OrderBy(x => x.DisplayOrder)
                        .ToList();

                    var orderItemMealSelections = new List<OrderItemMealSelection>();
                    var orderItemAddonSelections = new List<OrderItemAddonSelection>();

                    var requestMealSelections = item.MealSelections ;
                    var requestAddonSelections = item.AddonSelections ?? new List<OrderItemAddonSelectionRequestDto>();

                    var consumedMealSelections = 0;

                    foreach (var rule in activeRules)
                    {
                        var selectedForRule = requestMealSelections
                            .Where(x => x.PackageSelectionRuleId == rule.Id)
                            .ToList();

                        if (rule.IsRequired && selectedForRule.Count < rule.MinSelections)
                        {
                            return Results.BadRequest(new
                            {
                                message = $"Rule '{rule.Title}' requires at least {rule.MinSelections} selection(s)."
                            });
                        }

                        if (selectedForRule.Count > rule.MaxSelections)
                        {
                            return Results.BadRequest(new
                            {
                                message = $"Rule '{rule.Title}' allows only up to {rule.MaxSelections} selection(s)."
                            });
                        }

                        foreach (var selected in selectedForRule)
                        {
                            var option = rule.Options.FirstOrDefault(x =>
                                x.MealId == selected.MealId &&
                                x.IsActive &&
                                x.Meal.IsAvailable);

                            if (option is null)
                            {
                                return Results.BadRequest(new
                                {
                                    message = $"Meal id {selected.MealId} is not allowed for rule '{rule.Title}' in size '{selectedSize.Label}'."
                                });
                            }

                            orderItemMealSelections.Add(new OrderItemMealSelection
                            {
                                PackageSelectionRuleId = rule.Id,
                                MealId = option.MealId,
                                RuleTitleSnapshot = rule.Title,
                                MealNameSnapshot = option.Meal.Name,
                                AdditionalPrice = option.AdditionalPrice
                            });

                            consumedMealSelections++;
                        }
                    }

                    if (consumedMealSelections != requestMealSelections.Count)
                    {
                        return Results.BadRequest(new
                        {
                            message = "One or more meal selections do not match the selected package size rules."
                        });
                    }

                    foreach (var addonSelection in requestAddonSelections)
                    {
                        var addon = package.Addons.FirstOrDefault(x => x.Id == addonSelection.PackageAddonId && x.IsAvailable);

                        if (addon is null)
                        {
                            return Results.BadRequest(new
                            {
                                message = $"Addon id {addonSelection.PackageAddonId} is not valid for package '{package.Title}'."
                            });
                        }

                        orderItemAddonSelections.Add(new OrderItemAddonSelection
                        {
                            PackageAddonId = addon.Id,
                            AddonNameSnapshot = addon.Name,
                            AdditionalPrice = addon.Price
                        });
                    }

                    var packageOrderItem = new OrderItem
                    {
                        ItemType = OrderItemType.Package,
                        PackageId = package.Id,
                        PackageSizeId = selectedSize.Id,
                        MealId = null,
                        PackageTitleSnapshot = package.Title,
                        SizeLabelSnapshot = selectedSize.Label,
                        BaseUnitPrice = selectedSize.Price,
                        Quantity = item.Quantity,
                        MealSelections = orderItemMealSelections,
                        AddonSelections = orderItemAddonSelections
                    };

                    subTotal += packageOrderItem.LineTotal;
                    orderItems.Add(packageOrderItem);
                }

                var discountAmount = 0m;
                var deliveryFee = 0m;
                var taxAmount = Math.Round(subTotal * 0.12m, 2);
                var grandTotal = subTotal + deliveryFee + taxAmount - discountAmount;

                var orderedAtUtc = DateTime.UtcNow;

                var order = new Order
                {
                    OrderNumber = GenerateOrderNumber(),
                    CustomerProfileId = customerProfile.Id,
                    ContactNameSnapshot = request.ContactInfo.FullName?.Trim() ?? string.Empty,
                    ContactEmailSnapshot = request.ContactInfo.EmailAddress?.Trim() ?? string.Empty,
                    ContactMobileSnapshot = request.ContactInfo.MobileNumber?.Trim() ?? string.Empty,
                    Address = address,
                    OrderedAtUtc = orderedAtUtc,
                    DeliveryDate = request.DeliverySchedule.DeliveryDate.Date,
                    DeliveryTimeSlot = request.DeliverySchedule.TimeSlot,
                    Status = OrderStatus.Confirmed,
                    PaymentMethod = paymentMethod,
                    PaymentStatus = PaymentStatus.Pending,
                    CustomerNotes = request.CustomerNotes,
                    SpecialInstructions = request.SpecialInstructions,
                    PromoCodeApplied = request.PromoCode,
                    SubTotal = subTotal,
                    DeliveryFee = deliveryFee,
                    TaxAmount = taxAmount,
                    DiscountAmount = discountAmount,
                    GrandTotal = grandTotal,
                    Items = orderItems
                };

                db.Orders.Add(order);
                await db.SaveChangesAsync();

                var estimatedDeliveryUtc = BuildEstimatedDeliveryUtc(
                    request.DeliverySchedule.DeliveryDate,
                    request.DeliverySchedule.TimeSlot);

                var response = new PlaceOrderResponse
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    Status = order.Status.ToString(),
                    OrderedAtUtc = order.OrderedAtUtc,
                    EstimatedDeliveryTimeUtc = estimatedDeliveryUtc,
                    SubTotal = order.SubTotal,
                    DeliveryFee = order.DeliveryFee,
                    TaxAmount = order.TaxAmount,
                    DiscountAmount = order.DiscountAmount,
                    GrandTotal = order.GrandTotal
                };

                return Results.Created($"/api/customer/orders/{order.Id}/tracking", response);
            });


            //Get order tracking details
            app.MapGet("/api/customer/orders/{id:int}/tracking", async (int id, CmsDbContext db) =>
            {
                var order = await db.Orders
                    .AsNoTracking()
                    .Include(x => x.Address)
                    .Include(x => x.Items)
                        .ThenInclude(x => x.MealSelections)
                    .Include(x => x.Items)
                        .ThenInclude(x => x.AddonSelections)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (order is null)
                {
                    return Results.NotFound(new { message = $"Order with id {id} was not found." });
                }

                var response = new GetOrderTrackingResponse
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    Status = order.Status.ToString(),
                    OrderedAtUtc = order.OrderedAtUtc,
                    EstimatedDeliveryTimeUtc = BuildEstimatedDeliveryUtc(order.DeliveryDate, order.DeliveryTimeSlot),

                    DeliveryAddress = order.Address is null
                        ? new DeliveryAddressDto()
                        : new DeliveryAddressDto
                        {
                            StreetAddress = order.Address.StreetAddress,
                            Barangay = order.Address.Barangay,
                            City = order.Address.City,
                            Landmark = order.Address.Landmark,
                            Latitude = order.Address.Latitude,
                            Longitude = order.Address.Longitude
                        },

                    Rider = null,

                    AmountPaid = order.GrandTotal,

                    Items = order.Items
                        .OrderBy(x => x.Id)
                        .Select(x => new TrackingOrderItemDto
                        {
                            OrderItemId = x.Id,
                            PackageTitle = x.PackageTitleSnapshot,
                            SizeLabel = x.SizeLabelSnapshot,
                            Quantity = x.Quantity,
                            UnitPrice = x.UnitPrice,
                            LineTotal = x.LineTotal,
                            SelectedMeals = x.MealSelections
                                .OrderBy(m => m.Id)
                                .Select(m => m.MealNameSnapshot)
                                .ToList(),
                            SelectedAddons = x.AddonSelections
                                .OrderBy(a => a.Id)
                                .Select(a => a.AddonNameSnapshot)
                                .ToList()
                        })
                        .ToList(),

                    Steps = BuildTrackingSteps(order)
                };

                return Results.Ok(response);
            });

            //GET list of meals with optional filtering by category slug and search term for customer menu page
            app.MapGet("/api/customer/meals", async (
                CmsDbContext db,
                string? categorySlug,
                string? search) =>
            {
                var query = db.Meals
                    .AsNoTracking()
                    .Include(x => x.MenuCategory)
                    .Where(x => x.IsAvailable);

                if (!string.IsNullOrWhiteSpace(categorySlug))
                {
                    var normalizedSlug = categorySlug.Trim().ToLower();
                    query = query.Where(x => x.MenuCategory.Slug.ToLower() == normalizedSlug);
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var normalizedSearch = search.Trim().ToLower();
                    query = query.Where(x =>
                        x.Name.ToLower().Contains(normalizedSearch) ||
                        x.Description.ToLower().Contains(normalizedSearch));
                }

                var meals = await query
                    .OrderBy(x => x.Name)
                    .Select(x => new CustomerMealDto
                    {
                        Id = x.Id,
                        CategoryId = x.MenuCategoryId,
                        CategoryName = x.MenuCategory.Name,
                        CategorySlug = x.MenuCategory.Slug,
                        Name = x.Name,
                        Description = x.Description,
                        MealType = x.MealType.ToString(),
                        BasePrice = x.BasePrice,
                        MinOrderQuantity = x.MinOrderQuantity,
                        ImageUrl = x.ImageUrl,
                        IsAvailable = x.IsAvailable
                    })
                    .ToListAsync();

                return Results.Ok(new GetCustomerMealsResponse
                {
                    Meals = meals
                });
            });


            //GET meal details by id for customer menu page
            app.MapGet("/api/customer/meals/{id:int}", async (int id, CmsDbContext db) =>
            {
                var meal = await db.Meals
                    .AsNoTracking()
                    .Where(x => x.Id == id && x.IsAvailable)
                    .Include(x => x.MenuCategory)
                    .Select(x => new CustomerMealDto
                    {
                        Id = x.Id,
                        CategoryId = x.MenuCategoryId,
                        CategoryName = x.MenuCategory.Name,
                        CategorySlug = x.MenuCategory.Slug,
                        Name = x.Name,
                        Description = x.Description,
                        MealType = x.MealType.ToString(),
                        BasePrice = x.BasePrice,
                        MinOrderQuantity = x.MinOrderQuantity,
                        ImageUrl = x.ImageUrl,
                        IsAvailable = x.IsAvailable
                    })
                    .FirstOrDefaultAsync();

                return meal is not null
                    ? Results.Ok(meal)
                    : Results.NotFound(new { message = $"Meal with id {id} was not found." });
            });

            //POST customer registration and login endpoints
            app.MapPost("/api/customer/auth/register", async (
                CustomerRegisterRequest request,
                CmsDbContext db,
                IPasswordHasher<AppUser> passwordHasher,
                IEmailSender emailSender) =>
            {
                var normalizedUserName = request.UserName?.Trim() ?? string.Empty;
                var normalizedEmail = request.Email?.Trim() ?? string.Empty;

                if (!IsValidUsername(normalizedUserName))
                {
                    return Results.BadRequest(new
                    {
                        message = "Username must be 4-20 characters and contain only letters, numbers, underscore, or dot."
                    });
                }

                if (!IsValidEmail(normalizedEmail))
                {
                    return Results.BadRequest(new
                    {
                        message = "Please enter a valid email address."
                    });
                }

                if (!IsStrongPassword(request.Password))
                {
                    return Results.BadRequest(new
                    {
                        message = "Password must be at least 8 characters and include uppercase, lowercase, number, and symbol."
                    });
                }

                if (string.IsNullOrWhiteSpace(request.FirstName))
                {
                    return Results.BadRequest(new { message = "First name is required." });
                }

                if (string.IsNullOrWhiteSpace(request.LastName))
                {
                    return Results.BadRequest(new { message = "Last name is required." });
                }

                var userNameExists = await db.AppUsers.AnyAsync(x =>
                    x.UserName.ToLower() == normalizedUserName.ToLower());

                if (userNameExists)
                {
                    return Results.BadRequest(new
                    {
                        message = $"Username '{normalizedUserName}' is already taken."
                    });
                }

                var emailExists = await db.AppUsers.AnyAsync(x =>
                    x.Email.ToLower() == normalizedEmail.ToLower());

                if (emailExists)
                {
                    return Results.BadRequest(new
                    {
                        message = $"Email '{normalizedEmail}' is already registered."
                    });
                }

                var verificationCode = GenerateVerificationCode();

                var user = new AppUser
                {
                    UserName = normalizedUserName,
                    Email = normalizedEmail,
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    Role = UserRole.Customer,
                    IsActive = true,
                    IsEmailVerified = false,
                    EmailVerificationCode = verificationCode,
                    EmailVerificationCodeExpiresAtUtc = DateTime.UtcNow.AddMinutes(10)
                };

                user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

                db.AppUsers.Add(user);
                await db.SaveChangesAsync();

                var profile = new CustomerProfile
                {
                    AppUserId = user.Id,
                    MobileNumber = request.MobileNumber?.Trim() ?? string.Empty
                };

                db.CustomerProfiles.Add(profile);
                await db.SaveChangesAsync();

                await emailSender.SendAsync(
                    user.Email,
                    "Verify your Pesyong account",
                    $"""
                        <div style="font-family:Arial,sans-serif;line-height:1.5">
                            <h2>Welcome to Pesyong!</h2>
                            <p>Your verification code is:</p>
                            <div style="font-size:32px;font-weight:bold;letter-spacing:6px;color:#d86b23">
                                {verificationCode}
                            </div>
                            <p>This code will expire in 10 minutes.</p>
                        </div>
                        """);

                return Results.Ok(new RegisterResponse
                {
                    RequiresEmailVerification = true,
                    Email = user.Email,
                    Message = "Registration successful. Please check your email for the verification code."
                });
            });

            //POST customer login endpoint
            app.MapPost("/api/customer/auth/login", async (
                CustomerLoginRequest request,
                CmsDbContext db,
                IPasswordHasher<AppUser> passwordHasher,
                IOptions<JwtOptions> jwtOptionsAccessor) =>
            {
                if (string.IsNullOrWhiteSpace(request.UserNameOrEmail))
                    return Results.BadRequest(new { message = "Username or email is required." });

                if (string.IsNullOrWhiteSpace(request.Password))
                    return Results.BadRequest(new { message = "Password is required." });

                var input = request.UserNameOrEmail.Trim();

                var user = await db.AppUsers
                    .FirstOrDefaultAsync(x =>
                        x.UserName == input || x.Email == input);

                if (user is null || !user.IsActive)
                    return Results.BadRequest(new { message = "Invalid login credentials." });

                if (user.Role != UserRole.Customer)
                    return Results.BadRequest(new { message = "This login is not a customer account." });

                if (!user.IsEmailVerified)
                    return Results.BadRequest(new { message = "Please verify your email before signing in." });

                var verifyResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
                if (verifyResult == PasswordVerificationResult.Failed)
                    return Results.BadRequest(new { message = "Invalid login credentials." });

                var profile = await db.CustomerProfiles
                    .FirstOrDefaultAsync(x => x.AppUserId == user.Id);

                if (profile is null)
                    return Results.BadRequest(new { message = "Customer profile was not found." });

                var tokenResult = CreateCustomerAuthResponse(user, profile, jwtOptionsAccessor.Value);

                return Results.Ok(tokenResult);
            });


            //POST verify email endpoint for customer email verification flow after registration
            app.MapPost("/api/customer/auth/verify-email", async (
                VerifyEmailRequest request,
                CmsDbContext db,
                IOptions<JwtOptions> jwtOptionsAccessor) =>
            {
                var email = request.Email?.Trim() ?? string.Empty;
                var code = request.Code?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
                {
                    return Results.BadRequest(new { message = "Email and code are required." });
                }

                var user = await db.AppUsers
                    .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());

                if (user is null)
                {
                    return Results.BadRequest(new { message = "Invalid verification request." });
                }

                if (user.IsEmailVerified)
                {
                    return Results.BadRequest(new { message = "Email is already verified." });
                }

                if (user.EmailVerificationCode != code)
                {
                    return Results.BadRequest(new { message = "Invalid verification code." });
                }

                if (!user.EmailVerificationCodeExpiresAtUtc.HasValue ||
                    user.EmailVerificationCodeExpiresAtUtc.Value < DateTime.UtcNow)
                {
                    return Results.BadRequest(new { message = "Verification code has expired." });
                }

                user.IsEmailVerified = true;
                user.EmailVerificationCode = null;
                user.EmailVerificationCodeExpiresAtUtc = null;

                await db.SaveChangesAsync();

                var profile = await db.CustomerProfiles
                    .FirstOrDefaultAsync(x => x.AppUserId == user.Id);

                if (profile is null)
                {
                    return Results.BadRequest(new { message = "Customer profile was not found." });
                }

                var authResponse = CreateCustomerAuthResponse(user, profile, jwtOptionsAccessor.Value);
                return Results.Ok(authResponse);
            });


            //POST resend verification code endpoint for customer email verification flow
            app.MapPost("/api/customer/auth/resend-code", async (
                ResendVerificationCodeRequest request,
                CmsDbContext db,
                IEmailSender emailSender) =>
            {
                var email = request.Email?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(email))
                {
                    return Results.BadRequest(new { message = "Email is required." });
                }

                var user = await db.AppUsers
                    .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());

                if (user is null)
                {
                    return Results.BadRequest(new { message = "Email was not found." });
                }

                if (user.IsEmailVerified)
                {
                    return Results.BadRequest(new { message = "Email is already verified." });
                }

                var code = GenerateVerificationCode();
                user.EmailVerificationCode = code;
                user.EmailVerificationCodeExpiresAtUtc = DateTime.UtcNow.AddMinutes(10);

                await db.SaveChangesAsync();

                await emailSender.SendAsync(
                    user.Email,
                    "Your Pesyong verification code",
                    $"""
                        <div style="font-family:Arial,sans-serif;line-height:1.5">
                            <h2>Verify your Pesyong account</h2>
                            <p>Your new verification code is:</p>
                            <div style="font-size:32px;font-weight:bold;letter-spacing:6px;color:#d86b23">
                                {code}
                            </div>
                            <p>This code will expire in 10 minutes.</p>
                        </div>
                        """);

                return Results.Ok(new
                {
                    message = "A new verification code was sent to your email."
                });
            });

            //GET current authenticated customer details
            app.MapGet("/api/customer/auth/me", async (
                ClaimsPrincipal claims,
                CmsDbContext db) =>
            {
                var appUserIdClaim = claims.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(appUserIdClaim, out var appUserId))
                    return Results.Unauthorized();

                var result = await db.AppUsers
                    .Where(x => x.Id == appUserId && x.Role == UserRole.Customer)
                    .Join(
                        db.CustomerProfiles,
                        user => user.Id,
                        profile => profile.AppUserId,
                        (user, profile) => new CustomerMeResponse
                        {
                            AppUserId = user.Id,
                            CustomerProfileId = profile.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            FullName = $"{user.FirstName} {user.LastName}".Trim(),
                            MobileNumber = profile.MobileNumber
                        })
                    .FirstOrDefaultAsync();

                return result is not null
                    ? Results.Ok(result)
                    : Results.NotFound(new { message = "Customer profile was not found." });
            }).RequireAuthorization();


            //PUT update current authenticated customer profile details
            app.MapPut("/api/customer/profile", async (
                ClaimsPrincipal claims,
                UpdateCustomerProfileRequest request,
                CmsDbContext db) =>
            {
                var appUserIdClaim = claims.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(appUserIdClaim, out var appUserId))
                    return Results.Unauthorized();

                if (string.IsNullOrWhiteSpace(request.FirstName))
                    return Results.BadRequest(new { message = "First name is required." });

                if (string.IsNullOrWhiteSpace(request.LastName))
                    return Results.BadRequest(new { message = "Last name is required." });

                var user = await db.AppUsers
                    .Include(x => x.CustomerProfile)
                    .FirstOrDefaultAsync(x => x.Id == appUserId && x.Role == UserRole.Customer);

                if (user is null || user.CustomerProfile is null)
                    return Results.NotFound(new { message = "Customer profile was not found." });

                user.FirstName = request.FirstName.Trim();
                user.LastName = request.LastName.Trim();
                user.CustomerProfile.MobileNumber = request.MobileNumber?.Trim() ?? string.Empty;
                user.UpdatedAtUtc = DateTime.UtcNow;
                user.CustomerProfile.UpdatedAtUtc = DateTime.UtcNow;

                await db.SaveChangesAsync();

                var response = new CustomerMeResponse
                {
                    AppUserId = user.Id,
                    CustomerProfileId = user.CustomerProfile.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    MobileNumber = user.CustomerProfile.MobileNumber
                };

                return Results.Ok(response);
            }).RequireAuthorization();


            //GET list of customer addresses for current authenticated customer
            app.MapGet("/api/customer/profile/addresses", async (
                ClaimsPrincipal claims,
                CmsDbContext db) =>
            {
                var appUserIdClaim = claims.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(appUserIdClaim, out var appUserId))
                    return Results.Unauthorized();

                var customerProfileId = await db.CustomerProfiles
                    .Where(x => x.AppUserId == appUserId)
                    .Select(x => (int?)x.Id)
                    .FirstOrDefaultAsync();

                if (!customerProfileId.HasValue)
                    return Results.NotFound(new { message = "Customer profile was not found." });

                var addresses = await db.Addresses
                    .AsNoTracking()
                    .Where(x => x.CustomerProfileId == customerProfileId.Value)
                    .OrderByDescending(x => x.IsDefault)
                    .ThenByDescending(x => x.Id)
                    .Select(x => new CustomerAddressDto
                    {
                        Id = x.Id,
                        StreetAddress = x.StreetAddress,
                        City = x.City,
                        Barangay = x.Barangay,
                        Landmark = x.Landmark,
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        IsDefault = x.IsDefault
                    })
                    .ToListAsync();

                return Results.Ok(addresses);
            }).RequireAuthorization();


            //POST add a new customer address for current authenticated customer
            app.MapPost("/api/customer/profile/addresses", async (
                ClaimsPrincipal claims,
                SaveCustomerAddressRequest request,
                CmsDbContext db) =>
            {
                var appUserIdClaim = claims.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(appUserIdClaim, out var appUserId))
                    return Results.Unauthorized();

                var customerProfileId = await db.CustomerProfiles
                    .Where(x => x.AppUserId == appUserId)
                    .Select(x => (int?)x.Id)
                    .FirstOrDefaultAsync();

                if (!customerProfileId.HasValue)
                    return Results.NotFound(new { message = "Customer profile was not found." });

                if (string.IsNullOrWhiteSpace(request.StreetAddress))
                    return Results.BadRequest(new { message = "Street address is required." });

                if (string.IsNullOrWhiteSpace(request.City))
                    return Results.BadRequest(new { message = "City is required." });

                if (string.IsNullOrWhiteSpace(request.Barangay))
                    return Results.BadRequest(new { message = "Barangay is required." });

                if (request.IsDefault)
                {
                    var existingDefaults = await db.Addresses
                        .Where(x => x.CustomerProfileId == customerProfileId.Value && x.IsDefault)
                        .ToListAsync();

                    foreach (var item in existingDefaults)
                    {
                        item.IsDefault = false;
                    }
                }

                var address = new Address
                {
                    CustomerProfileId = customerProfileId.Value,
                    StreetAddress = request.StreetAddress.Trim(),
                    City = request.City.Trim(),
                    Barangay = request.Barangay.Trim(),
                    Landmark = request.Landmark?.Trim() ?? string.Empty,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    IsDefault = request.IsDefault
                };

                db.Addresses.Add(address);
                await db.SaveChangesAsync();

                return Results.Ok(new CustomerAddressDto
                {
                    Id = address.Id,
                    StreetAddress = address.StreetAddress,
                    City = address.City,
                    Barangay = address.Barangay,
                    Landmark = address.Landmark,
                    Latitude = address.Latitude,
                    Longitude = address.Longitude,
                    IsDefault = address.IsDefault
                });
            }).RequireAuthorization();


            //PUT update an existing customer address by id for current authenticated customer
            app.MapPut("/api/customer/profile/addresses/{id:int}", async (
                int id,
                ClaimsPrincipal claims,
                SaveCustomerAddressRequest request,
                CmsDbContext db) =>
            {
                var appUserIdClaim = claims.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(appUserIdClaim, out var appUserId))
                    return Results.Unauthorized();

                var customerProfileId = await db.CustomerProfiles
                    .Where(x => x.AppUserId == appUserId)
                    .Select(x => (int?)x.Id)
                    .FirstOrDefaultAsync();

                if (!customerProfileId.HasValue)
                    return Results.NotFound(new { message = "Customer profile was not found." });

                var address = await db.Addresses
                    .FirstOrDefaultAsync(x => x.Id == id && x.CustomerProfileId == customerProfileId.Value);

                if (address is null)
                    return Results.NotFound(new { message = $"Address with id {id} was not found." });

                if (string.IsNullOrWhiteSpace(request.StreetAddress))
                    return Results.BadRequest(new { message = "Street address is required." });

                if (string.IsNullOrWhiteSpace(request.City))
                    return Results.BadRequest(new { message = "City is required." });

                if (string.IsNullOrWhiteSpace(request.Barangay))
                    return Results.BadRequest(new { message = "Barangay is required." });

                if (request.IsDefault)
                {
                    var existingDefaults = await db.Addresses
                        .Where(x => x.CustomerProfileId == customerProfileId.Value && x.IsDefault && x.Id != id)
                        .ToListAsync();

                    foreach (var item in existingDefaults)
                    {
                        item.IsDefault = false;
                    }
                }

                address.StreetAddress = request.StreetAddress.Trim();
                address.City = request.City.Trim();
                address.Barangay = request.Barangay.Trim();
                address.Landmark = request.Landmark?.Trim() ?? string.Empty;
                address.Latitude = request.Latitude;
                address.Longitude = request.Longitude;
                address.IsDefault = request.IsDefault;
                address.UpdatedAtUtc = DateTime.UtcNow;

                await db.SaveChangesAsync();

                return Results.Ok(new CustomerAddressDto
                {
                    Id = address.Id,
                    StreetAddress = address.StreetAddress,
                    City = address.City,
                    Barangay = address.Barangay,
                    Landmark = address.Landmark,
                    Latitude = address.Latitude,
                    Longitude = address.Longitude,
                    IsDefault = address.IsDefault
                });
            }).RequireAuthorization();


            //PUT set an existing customer address as default by id for current authenticated customer
            app.MapPut("/api/customer/profile/addresses/{id:int}/default", async (
                int id,
                ClaimsPrincipal claims,
                CmsDbContext db) =>
            {
                var appUserIdClaim = claims.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(appUserIdClaim, out var appUserId))
                    return Results.Unauthorized();

                var customerProfileId = await db.CustomerProfiles
                    .Where(x => x.AppUserId == appUserId)
                    .Select(x => (int?)x.Id)
                    .FirstOrDefaultAsync();

                if (!customerProfileId.HasValue)
                    return Results.NotFound(new { message = "Customer profile was not found." });

                var addresses = await db.Addresses
                    .Where(x => x.CustomerProfileId == customerProfileId.Value)
                    .ToListAsync();

                var target = addresses.FirstOrDefault(x => x.Id == id);
                if (target is null)
                    return Results.NotFound(new { message = $"Address with id {id} was not found." });

                foreach (var address in addresses)
                {
                    address.IsDefault = address.Id == id;
                    address.UpdatedAtUtc = DateTime.UtcNow;
                }

                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Default address updated." });
            }).RequireAuthorization();


            //DELETE remove an existing customer address by id for current authenticated customer
            app.MapDelete("/api/customer/profile/addresses/{id:int}", async (
                int id,
                ClaimsPrincipal claims,
                CmsDbContext db) =>
            {
                var appUserIdClaim = claims.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(appUserIdClaim, out var appUserId))
                    return Results.Unauthorized();

                var customerProfileId = await db.CustomerProfiles
                    .Where(x => x.AppUserId == appUserId)
                    .Select(x => (int?)x.Id)
                    .FirstOrDefaultAsync();

                if (!customerProfileId.HasValue)
                    return Results.NotFound(new { message = "Customer profile was not found." });

                var address = await db.Addresses
                    .FirstOrDefaultAsync(x => x.Id == id && x.CustomerProfileId == customerProfileId.Value);

                if (address is null)
                    return Results.NotFound(new { message = $"Address with id {id} was not found." });

                var wasDefault = address.IsDefault;

                db.Addresses.Remove(address);
                await db.SaveChangesAsync();

                if (wasDefault)
                {
                    var replacement = await db.Addresses
                        .Where(x => x.CustomerProfileId == customerProfileId.Value)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefaultAsync();

                    if (replacement is not null)
                    {
                        replacement.IsDefault = true;
                        replacement.UpdatedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync();
                    }
                }

                return Results.Ok(new { message = "Address deleted." });
            }).RequireAuthorization();


            //GET list of orders for current authenticated customer
            app.MapGet("/api/customer/orders/my", async (
                ClaimsPrincipal claims,
                CmsDbContext db) =>
            {
                var appUserIdClaim = claims.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(appUserIdClaim, out var appUserId))
                    return Results.Unauthorized();

                var customerProfileId = await db.CustomerProfiles
                    .Where(x => x.AppUserId == appUserId)
                    .Select(x => (int?)x.Id)
                    .FirstOrDefaultAsync();

                if (!customerProfileId.HasValue)
                    return Results.NotFound(new { message = "Customer profile was not found." });

                var orders = await db.Orders
                    .AsNoTracking()
                    .Where(x => x.CustomerProfileId == customerProfileId.Value)
                    .Include(x => x.Items)
                    .OrderByDescending(x => x.OrderedAtUtc)
                    .Select(x => new CustomerOrderListItemDto
                    {
                        Id = x.Id,
                        OrderNumber = x.OrderNumber,
                        OrderedAtUtc = x.OrderedAtUtc,
                        DeliveryDate = x.DeliveryDate,
                        DeliveryTimeSlot = x.DeliveryTimeSlot,
                        Status = x.Status.ToString(),
                        PaymentStatus = x.PaymentStatus.ToString(),
                        GrandTotal = x.GrandTotal,
                        ItemCount = x.Items.Count
                    })
                    .ToListAsync();

                return Results.Ok(orders);
            }).RequireAuthorization();


            //================================= ADMIN ENDPOINTS ================================= //

            //GET dashboard stats
            app.MapGet("/api/admin/dashboard/stats", async (CmsDbContext db) =>
            {
                var totalOrders = await db.Orders.CountAsync();
                var pendingOrders = await db.Orders.CountAsync(x => x.Status == OrderStatus.Pending);
                var confirmedOrders = await db.Orders.CountAsync(x => x.Status == OrderStatus.Confirmed);
                var deliveredOrders = await db.Orders.CountAsync(x => x.Status == OrderStatus.Delivered);

                var totalCustomers = await db.CustomerProfiles.CountAsync();
                var totalPackages = await db.Packages.CountAsync(x => x.IsAvailable);
                var totalMeals = await db.Meals.CountAsync(x => x.IsAvailable);

                var totalRevenue = await db.Orders
                    .Where(x => x.Status != OrderStatus.Cancelled)
                    .SumAsync(x => (decimal?)x.GrandTotal) ?? 0m;

                var averageOrderValue = totalOrders > 0
                    ? Math.Round(totalRevenue / totalOrders, 2)
                    : 0m;

                var response = new DashboardStatsDto
                {
                    TotalOrders = totalOrders,
                    PendingOrders = pendingOrders,
                    ConfirmedOrders = confirmedOrders,
                    DeliveredOrders = deliveredOrders,
                    TotalCustomers = totalCustomers,
                    TotalPackages = totalPackages,
                    TotalMeals = totalMeals,
                    TotalRevenue = totalRevenue,
                    AverageOrderValue = averageOrderValue
                };

                return Results.Ok(response);
            });

            ////GET list of packages for admin package management page
            //app.MapGet("/api/admin/packages", async (CmsDbContext db) =>
            //{
            //    var packages = await db.Packages
            //        .AsNoTracking()
            //        .Include(x => x.MenuCategory)
            //        .Include(x => x.Sizes)
            //        .Include(x => x.Addons)
            //        .Include(x => x.SelectionRules)
            //        .OrderBy(x => x.Title)
            //        .Select(x => new PackageListItemDto
            //        {
            //            Id = x.Id,
            //            Title = x.Title,
            //            CategoryName = x.MenuCategory.Name,
            //            Badge = x.Badge,
            //            Notice = x.Notice,
            //            ServesLabel = x.ServesLabel,
            //            Rating = x.Rating,
            //            ReviewCount = x.ReviewCount,
            //            IsAvailable = x.IsAvailable,
            //            IsCustomizable = x.IsCustomizable,
            //            SizeCount = x.Sizes.Count,
            //            AddonCount = x.Addons.Count,
            //            SelectionRuleCount = x.SelectionRules.Count
            //        })
            //        .ToListAsync();

            //    return Results.Ok(packages);
            //});

            ////GET package details by id for admin package management page
            //app.MapGet("/api/admin/packages/{id:int}", async (int id, CmsDbContext db) =>
            //{
            //    var package = await db.Packages
            //        .AsNoTracking()
            //        .Where(x => x.Id == id)
            //        .Include(x => x.MenuCategory)
            //        .Include(x => x.Sizes)
            //        .Include(x => x.Addons)
            //        .Include(x => x.SelectionRules)
            //            .ThenInclude(x => x.Options)
            //                .ThenInclude(x => x.Meal)
            //        .Select(x => new AdminPackageDetailsDto
            //        {
            //            Id = x.Id,
            //            CategoryId = x.MenuCategoryId,
            //            CategoryName = x.MenuCategory.Name,

            //            Title = x.Title,
            //            Description = x.Description,
            //            CardSummary = x.CardSummary,
            //            Badge = x.Badge,
            //            Notice = x.Notice,
            //            ServesLabel = x.ServesLabel,
            //            InclusionText = x.InclusionText,
            //            ImageUrl = x.ImageUrl,

            //            Rating = x.Rating,
            //            ReviewCount = x.ReviewCount,

            //            IsAvailable = x.IsAvailable,
            //            IsCustomizable = x.IsCustomizable,

            //            Sizes = x.Sizes
            //                .OrderBy(s => s.PaxCount)
            //                .Select(s => new AdminPackageSizeDto
            //                {
            //                    Id = s.Id,
            //                    Label = s.Label,
            //                    Subtitle = s.Subtitle,
            //                    PaxCount = s.PaxCount,
            //                    Price = s.Price
            //                })
            //                .ToList(),

            //            Addons = x.Addons
            //                .OrderBy(a => a.Name)
            //                .Select(a => new AdminPackageAddonDto
            //                {
            //                    Id = a.Id,
            //                    Name = a.Name,
            //                    Description = a.Description,
            //                    Price = a.Price,
            //                    IsAvailable = a.IsAvailable
            //                })
            //                .ToList(),

            //            SelectionRules = x.SelectionRules
            //                .OrderBy(r => r.DisplayOrder)
            //                .Select(r => new AdminPackageSelectionRuleDto
            //                {
            //                    Id = r.Id,
            //                    Title = r.Title,
            //                    Description = r.Description,
            //                    SelectionType = r.SelectionType.ToString(),
            //                    AllowedMealType = r.AllowedMealType.ToString(),
            //                    MinSelections = r.MinSelections,
            //                    MaxSelections = r.MaxSelections,
            //                    IsRequired = r.IsRequired,
            //                    DisplayOrder = r.DisplayOrder,
            //                    Options = r.Options
            //                        .OrderBy(o => o.Meal.Name)
            //                        .Select(o => new AdminPackageSelectionOptionDto
            //                        {
            //                            Id = o.Id,
            //                            MealId = o.MealId,
            //                            MealName = o.Meal.Name,
            //                            MealType = o.Meal.MealType.ToString(),
            //                            AdditionalPrice = o.AdditionalPrice,
            //                            IsDefault = o.IsDefault
            //                        })
            //                        .ToList()
            //                })
            //                .ToList()
            //        })
            //        .FirstOrDefaultAsync();

            //    return package is not null
            //        ? Results.Ok(package)
            //        : Results.NotFound(new { message = $"Package with id {id} was not found." });
            //});

            ////GET list of orders for admin order management page
            //app.MapGet("/api/admin/orders", async (CmsDbContext db) =>
            //{
            //    var orders = await db.Orders
            //        .AsNoTracking()
            //        .Include(x => x.CustomerProfile)
            //            .ThenInclude(x => x.AppUser)
            //        .Include(x => x.Items)
            //        .OrderByDescending(x => x.OrderedAtUtc)
            //        .Select(x => new AdminOrderListItemDto
            //        {
            //            Id = x.Id,
            //            OrderNumber = x.OrderNumber,
            //            CustomerProfileId = x.CustomerProfileId,

            //            CustomerName = !string.IsNullOrWhiteSpace(x.ContactNameSnapshot)
            //                ? x.ContactNameSnapshot
            //                : $"{x.CustomerProfile.AppUser.FirstName} {x.CustomerProfile.AppUser.LastName}".Trim(),

            //            CustomerEmail = !string.IsNullOrWhiteSpace(x.ContactEmailSnapshot)
            //                ? x.ContactEmailSnapshot
            //                : x.CustomerProfile.AppUser.Email,

            //            CustomerMobileNumber = !string.IsNullOrWhiteSpace(x.ContactMobileSnapshot)
            //                ? x.ContactMobileSnapshot
            //                : x.CustomerProfile.MobileNumber,

            //            OrderedAtUtc = x.OrderedAtUtc,
            //            DeliveryDate = x.DeliveryDate,
            //            DeliveryTimeSlot = x.DeliveryTimeSlot,
            //            Status = x.Status.ToString(),
            //            PaymentMethod = x.PaymentMethod.ToString(),
            //            PaymentStatus = x.PaymentStatus.ToString(),
            //            ItemCount = x.Items.Count,
            //            GrandTotal = x.GrandTotal
            //        })
            //        .ToListAsync();

            //    return Results.Ok(orders);
            //});

            ////GET order details by id for admin order management page
            //app.MapGet("/api/admin/orders/{id:int}", async (int id, CmsDbContext db) =>
            //{
            //    var order = await db.Orders
            //        .AsNoTracking()
            //        .Where(x => x.Id == id)
            //        .Include(x => x.CustomerProfile)
            //            .ThenInclude(x => x.AppUser)
            //        .Include(x => x.Address)
            //        .Include(x => x.Items)
            //            .ThenInclude(x => x.MealSelections)
            //        .Include(x => x.Items)
            //            .ThenInclude(x => x.AddonSelections)
            //        .Select(x => new AdminOrderDetailsDto
            //        {
            //            Id = x.Id,
            //            OrderNumber = x.OrderNumber,
            //            CustomerProfileId = x.CustomerProfileId,

            //            CustomerName = !string.IsNullOrWhiteSpace(x.ContactNameSnapshot)
            //                ? x.ContactNameSnapshot
            //                : $"{x.CustomerProfile.AppUser.FirstName} {x.CustomerProfile.AppUser.LastName}".Trim(),

            //            CustomerEmail = !string.IsNullOrWhiteSpace(x.ContactEmailSnapshot)
            //                ? x.ContactEmailSnapshot
            //                : x.CustomerProfile.AppUser.Email,

            //            CustomerMobileNumber = !string.IsNullOrWhiteSpace(x.ContactMobileSnapshot)
            //                ? x.ContactMobileSnapshot
            //                : x.CustomerProfile.MobileNumber,

            //            OrderedAtUtc = x.OrderedAtUtc,
            //            DeliveryDate = x.DeliveryDate,
            //            DeliveryTimeSlot = x.DeliveryTimeSlot,
            //            Status = x.Status.ToString(),
            //            PaymentMethod = x.PaymentMethod.ToString(),
            //            PaymentStatus = x.PaymentStatus.ToString(),
            //            CustomerNotes = x.CustomerNotes,
            //            SpecialInstructions = x.SpecialInstructions,
            //            PromoCodeApplied = x.PromoCodeApplied,

            //            DeliveryAddress = x.Address == null
            //                ? null
            //                : new AdminOrderAddressDto
            //                {
            //                    StreetAddress = x.Address.StreetAddress,
            //                    Barangay = x.Address.Barangay,
            //                    City = x.Address.City,
            //                    Landmark = x.Address.Landmark,
            //                    Latitude = x.Address.Latitude,
            //                    Longitude = x.Address.Longitude
            //                },

            //            SubTotal = x.SubTotal,
            //            DeliveryFee = x.DeliveryFee,
            //            TaxAmount = x.TaxAmount,
            //            DiscountAmount = x.DiscountAmount,
            //            GrandTotal = x.GrandTotal,

            //            Items = x.Items
            //                .OrderBy(i => i.Id)
            //                .Select(i => new AdminOrderItemDto
            //                {
            //                    Id = i.Id,
            //                    PackageTitle = i.PackageTitleSnapshot,
            //                    SizeLabel = i.SizeLabelSnapshot,
            //                    Quantity = i.Quantity,
            //                    UnitPrice = i.UnitPrice,
            //                    LineTotal = i.LineTotal,
            //                    SelectedMeals = i.MealSelections
            //                        .OrderBy(m => m.Id)
            //                        .Select(m => m.MealNameSnapshot)
            //                        .ToList(),
            //                    SelectedAddons = i.AddonSelections
            //                        .OrderBy(a => a.Id)
            //                        .Select(a => a.AddonNameSnapshot)
            //                        .ToList()
            //                })
            //                .ToList()
            //        })
            //        .FirstOrDefaultAsync();

            //    return order is not null
            //        ? Results.Ok(order)
            //        : Results.NotFound(new { message = $"Order with id {id} was not found." });
            //});

            ////GET list of customers for admin customer management page
            //app.MapGet("/api/admin/customers", async (CmsDbContext db) =>
            //{
            //    var customers = await db.CustomerProfiles
            //        .AsNoTracking()
            //        .Include(x => x.AppUser)
            //        .Include(x => x.Orders)
            //        .OrderBy(x => x.AppUser.LastName)
            //        .ThenBy(x => x.AppUser.FirstName)
            //        .Select(x => new AdminCustomerListItemDto
            //        {
            //            CustomerProfileId = x.Id,
            //            AppUserId = x.AppUserId,
            //            FullName = $"{x.AppUser.FirstName} {x.AppUser.LastName}".Trim(),
            //            Email = x.AppUser.Email,
            //            MobileNumber = x.MobileNumber,
            //            TotalOrders = x.Orders.Count,
            //            TotalSpent = x.Orders
            //                .Where(o => o.Status != OrderStatus.Cancelled)
            //                .Sum(o => (decimal?)o.GrandTotal) ?? 0m,
            //            LastOrderDateUtc = x.Orders
            //                .OrderByDescending(o => o.OrderedAtUtc)
            //                .Select(o => (DateTime?)o.OrderedAtUtc)
            //                .FirstOrDefault()
            //        })
            //        .ToListAsync();

            //    return Results.Ok(customers);
            //});


            ////GET customer details by id for admin customer management page
            //app.MapGet("/api/admin/customers/{id:int}", async (int id, CmsDbContext db) =>
            //{
            //    var customer = await db.CustomerProfiles
            //        .AsNoTracking()
            //        .Where(x => x.Id == id)
            //        .Include(x => x.AppUser)
            //        .Include(x => x.Addresses)
            //        .Include(x => x.Orders)
            //        .Select(x => new AdminCustomerDetailsDto
            //        {
            //            CustomerProfileId = x.Id,
            //            AppUserId = x.AppUserId,

            //            FirstName = x.AppUser.FirstName,
            //            LastName = x.AppUser.LastName,
            //            FullName = $"{x.AppUser.FirstName} {x.AppUser.LastName}".Trim(),

            //            Email = x.AppUser.Email,
            //            MobileNumber = x.MobileNumber,
            //            IsActive = x.AppUser.IsActive,
            //            CreatedAtUtc = x.CreatedAtUtc,

            //            TotalOrders = x.Orders.Count,
            //            TotalSpent = x.Orders
            //                .Where(o => o.Status != OrderStatus.Cancelled)
            //                .Sum(o => (decimal?)o.GrandTotal) ?? 0m,

            //            Addresses = x.Addresses
            //                .OrderByDescending(a => a.IsDefault)
            //                .ThenBy(a => a.Id)
            //                .Select(a => new AdminCustomerAddressDto
            //                {
            //                    Id = a.Id,
            //                    StreetAddress = a.StreetAddress,
            //                    Barangay = a.Barangay,
            //                    City = a.City,
            //                    Landmark = a.Landmark,
            //                    IsDefault = a.IsDefault,
            //                    Latitude = a.Latitude,
            //                    Longitude = a.Longitude
            //                })
            //                .ToList(),

            //            RecentOrders = x.Orders
            //                .OrderByDescending(o => o.OrderedAtUtc)
            //                .Take(10)
            //                .Select(o => new AdminCustomerOrderSummaryDto
            //                {
            //                    OrderId = o.Id,
            //                    OrderNumber = o.OrderNumber,
            //                    OrderedAtUtc = o.OrderedAtUtc,
            //                    Status = o.Status.ToString(),
            //                    GrandTotal = o.GrandTotal
            //                })
            //                .ToList()
            //        })
            //        .FirstOrDefaultAsync();

            //    return customer is not null
            //        ? Results.Ok(customer)
            //        : Results.NotFound(new { message = $"Customer with id {id} was not found." });
            //});

            ////PUT update order status by id for admin order management page
            //app.MapPut("/api/admin/orders/{id:int}/status", async (int id, UpdateOrderStatusRequest request, CmsDbContext db) =>
            //{
            //    if (string.IsNullOrWhiteSpace(request.Status))
            //    {
            //        return Results.BadRequest(new { message = "Status is required." });
            //    }

            //    if (!TryResolveAdminOrderStatus(request.Status, out var newStatus))
            //    {
            //        return Results.BadRequest(new
            //        {
            //            message = $"Unsupported order status: '{request.Status}'. Allowed values are Confirmed, Preparing, OutForDelivery, Delivered, Cancelled."
            //        });
            //    }

            //    var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == id);

            //    if (order is null)
            //    {
            //        return Results.NotFound(new { message = $"Order with id {id} was not found." });
            //    }

            //    if (order.Status == OrderStatus.Cancelled)
            //    {
            //        return Results.BadRequest(new
            //        {
            //            message = "Cancelled orders can no longer be updated."
            //        });
            //    }

            //    if (order.Status == OrderStatus.Delivered && newStatus != OrderStatus.Delivered)
            //    {
            //        return Results.BadRequest(new
            //        {
            //            message = "Delivered orders can no longer be changed to another status."
            //        });
            //    }

            //    var previousStatus = order.Status;
            //    order.Status = newStatus;
            //    order.UpdatedAtUtc = DateTime.UtcNow;

            //    await db.SaveChangesAsync();

            //    var response = new UpdateOrderStatusResponse
            //    {
            //        OrderId = order.Id,
            //        OrderNumber = order.OrderNumber,
            //        PreviousStatus = previousStatus.ToString(),
            //        CurrentStatus = order.Status.ToString(),
            //        UpdatedAtUtc = order.UpdatedAtUtc ?? DateTime.UtcNow,
            //        Notes = request.Notes
            //    };

            //    return Results.Ok(response);
            //});


            ////POST create new package for admin package management page
            //app.MapPost("/api/admin/packages", async (SavePackageRequest request, CmsDbContext db) =>
            //{
            //    if (request.CategoryId <= 0)
            //    {
            //        return Results.BadRequest(new { message = "CategoryId is required." });
            //    }

            //    if (string.IsNullOrWhiteSpace(request.Title))
            //    {
            //        return Results.BadRequest(new { message = "Package title is required." });
            //    }

            //    var category = await db.MenuCategories
            //        .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            //    if (category is null)
            //    {
            //        return Results.BadRequest(new { message = $"Category with id {request.CategoryId} was not found." });
            //    }

            //    if (await db.Packages.AnyAsync(x => x.Title == request.Title))
            //    {
            //        return Results.BadRequest(new { message = $"A package with title '{request.Title}' already exists." });
            //    }

            //    var package = new Package
            //    {
            //        MenuCategoryId = request.CategoryId,
            //        Title = request.Title.Trim(),
            //        Description = request.Description.Trim(),
            //        CardSummary = request.CardSummary.Trim(),
            //        Badge = request.Badge.Trim(),
            //        Notice = request.Notice.Trim(),
            //        ServesLabel = request.ServesLabel.Trim(),
            //        InclusionText = request.InclusionText.Trim(),
            //        ImageUrl = request.ImageUrl.Trim(),
            //        Rating = request.Rating,
            //        ReviewCount = request.ReviewCount,
            //        IsAvailable = request.IsAvailable,
            //        IsCustomizable = request.IsCustomizable
            //    };

            //    foreach (var size in request.Sizes)
            //    {
            //        if (string.IsNullOrWhiteSpace(size.Label))
            //        {
            //            return Results.BadRequest(new { message = "Each package size must have a label." });
            //        }

            //        if (size.PaxCount <= 0)
            //        {
            //            return Results.BadRequest(new { message = $"Package size '{size.Label}' must have a pax count greater than zero." });
            //        }

            //        if (size.Price < 0)
            //        {
            //            return Results.BadRequest(new { message = $"Package size '{size.Label}' cannot have a negative price." });
            //        }

            //        package.Sizes.Add(new PackageSize
            //        {
            //            Label = size.Label.Trim(),
            //            Subtitle = size.Subtitle.Trim(),
            //            PaxCount = size.PaxCount,
            //            Price = size.Price
            //        });
            //    }

            //    foreach (var addon in request.Addons)
            //    {
            //        if (string.IsNullOrWhiteSpace(addon.Name))
            //        {
            //            return Results.BadRequest(new { message = "Each add-on must have a name." });
            //        }

            //        if (addon.Price < 0)
            //        {
            //            return Results.BadRequest(new { message = $"Add-on '{addon.Name}' cannot have a negative price." });
            //        }

            //        package.Addons.Add(new PackageAddon
            //        {
            //            Name = addon.Name.Trim(),
            //            Description = addon.Description.Trim(),
            //            Price = addon.Price,
            //            IsAvailable = addon.IsAvailable
            //        });
            //    }

            //    foreach (var ruleRequest in request.SelectionRules)
            //    {
            //        if (!TryResolvePackageSelectionType(ruleRequest.SelectionType, out var selectionType))
            //        {
            //            return Results.BadRequest(new
            //            {
            //                message = $"Invalid selection type '{ruleRequest.SelectionType}' for rule '{ruleRequest.Title}'."
            //            });
            //        }

            //        if (!TryResolveMealType(ruleRequest.AllowedMealType, out var allowedMealType))
            //        {
            //            return Results.BadRequest(new
            //            {
            //                message = $"Invalid meal type '{ruleRequest.AllowedMealType}' for rule '{ruleRequest.Title}'."
            //            });
            //        }

            //        if (string.IsNullOrWhiteSpace(ruleRequest.Title))
            //        {
            //            return Results.BadRequest(new { message = "Each selection rule must have a title." });
            //        }

            //        if (ruleRequest.MaxSelections < ruleRequest.MinSelections)
            //        {
            //            return Results.BadRequest(new
            //            {
            //                message = $"Rule '{ruleRequest.Title}' has MaxSelections lower than MinSelections."
            //            });
            //        }

            //        var rule = new PackageSelectionRule
            //        {
            //            Title = ruleRequest.Title.Trim(),
            //            Description = ruleRequest.Description.Trim(),
            //            SelectionType = selectionType,
            //            AllowedMealType = allowedMealType,
            //            MinSelections = ruleRequest.MinSelections,
            //            MaxSelections = ruleRequest.MaxSelections,
            //            IsRequired = ruleRequest.IsRequired,
            //            DisplayOrder = ruleRequest.DisplayOrder
            //        };

            //        foreach (var optionRequest in ruleRequest.Options)
            //        {
            //            var meal = await db.Meals.FirstOrDefaultAsync(x => x.Id == optionRequest.MealId);

            //            if (meal is null)
            //            {
            //                return Results.BadRequest(new
            //                {
            //                    message = $"Meal with id {optionRequest.MealId} was not found for rule '{ruleRequest.Title}'."
            //                });
            //            }

            //            if (meal.MealType != allowedMealType)
            //            {
            //                return Results.BadRequest(new
            //                {
            //                    message = $"Meal '{meal.Name}' does not match the allowed meal type '{allowedMealType}' for rule '{ruleRequest.Title}'."
            //                });
            //            }

            //            rule.Options.Add(new PackageSelectionOption
            //            {
            //                MealId = meal.Id,
            //                AdditionalPrice = optionRequest.AdditionalPrice,
            //                IsDefault = optionRequest.IsDefault
            //            });
            //        }

            //        package.SelectionRules.Add(rule);
            //    }

            //    db.Packages.Add(package);
            //    await db.SaveChangesAsync();

            //    var created = await db.Packages
            //        .AsNoTracking()
            //        .Where(x => x.Id == package.Id)
            //        .Include(x => x.MenuCategory)
            //        .Include(x => x.Sizes)
            //        .Include(x => x.Addons)
            //        .Include(x => x.SelectionRules)
            //            .ThenInclude(x => x.Options)
            //                .ThenInclude(x => x.Meal)
            //        .Select(x => new AdminPackageDetailsDto
            //        {
            //            Id = x.Id,
            //            CategoryId = x.MenuCategoryId,
            //            CategoryName = x.MenuCategory.Name,

            //            Title = x.Title,
            //            Description = x.Description,
            //            CardSummary = x.CardSummary,
            //            Badge = x.Badge,
            //            Notice = x.Notice,
            //            ServesLabel = x.ServesLabel,
            //            InclusionText = x.InclusionText,
            //            ImageUrl = x.ImageUrl,

            //            Rating = x.Rating,
            //            ReviewCount = x.ReviewCount,

            //            IsAvailable = x.IsAvailable,
            //            IsCustomizable = x.IsCustomizable,

            //            Sizes = x.Sizes
            //                .OrderBy(s => s.PaxCount)
            //                .Select(s => new AdminPackageSizeDto
            //                {
            //                    Id = s.Id,
            //                    Label = s.Label,
            //                    Subtitle = s.Subtitle,
            //                    PaxCount = s.PaxCount,
            //                    Price = s.Price
            //                })
            //                .ToList(),

            //            Addons = x.Addons
            //                .OrderBy(a => a.Name)
            //                .Select(a => new AdminPackageAddonDto
            //                {
            //                    Id = a.Id,
            //                    Name = a.Name,
            //                    Description = a.Description,
            //                    Price = a.Price,
            //                    IsAvailable = a.IsAvailable
            //                })
            //                .ToList(),

            //            SelectionRules = x.SelectionRules
            //                .OrderBy(r => r.DisplayOrder)
            //                .Select(r => new AdminPackageSelectionRuleDto
            //                {
            //                    Id = r.Id,
            //                    Title = r.Title,
            //                    Description = r.Description,
            //                    SelectionType = r.SelectionType.ToString(),
            //                    AllowedMealType = r.AllowedMealType.ToString(),
            //                    MinSelections = r.MinSelections,
            //                    MaxSelections = r.MaxSelections,
            //                    IsRequired = r.IsRequired,
            //                    DisplayOrder = r.DisplayOrder,
            //                    Options = r.Options
            //                        .OrderBy(o => o.Meal.Name)
            //                        .Select(o => new AdminPackageSelectionOptionDto
            //                        {
            //                            Id = o.Id,
            //                            MealId = o.MealId,
            //                            MealName = o.Meal.Name,
            //                            MealType = o.Meal.MealType.ToString(),
            //                            AdditionalPrice = o.AdditionalPrice,
            //                            IsDefault = o.IsDefault
            //                        })
            //                        .ToList()
            //                })
            //                .ToList()
            //        })
            //        .FirstOrDefaultAsync();

            //    return Results.Created($"/api/admin/packages/{package.Id}", created);
            //});


            ////PUT update existing package by id for admin package management page
            //app.MapPut("/api/admin/packages/{id:int}", async (int id, SavePackageRequest request, CmsDbContext db) =>
            //{
            //    if (request.CategoryId <= 0)
            //    {
            //        return Results.BadRequest(new { message = "CategoryId is required." });
            //    }

            //    if (string.IsNullOrWhiteSpace(request.Title))
            //    {
            //        return Results.BadRequest(new { message = "Package title is required." });
            //    }

            //    var package = await db.Packages
            //        .Include(x => x.Sizes)
            //        .Include(x => x.Addons)
            //        .Include(x => x.SelectionRules)
            //            .ThenInclude(x => x.Options)
            //        .FirstOrDefaultAsync(x => x.Id == id);

            //    if (package is null)
            //    {
            //        return Results.NotFound(new { message = $"Package with id {id} was not found." });
            //    }

            //    var category = await db.MenuCategories
            //        .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            //    if (category is null)
            //    {
            //        return Results.BadRequest(new { message = $"Category with id {request.CategoryId} was not found." });
            //    }

            //    var duplicateTitleExists = await db.Packages
            //        .AnyAsync(x => x.Id != id && x.Title == request.Title);

            //    if (duplicateTitleExists)
            //    {
            //        return Results.BadRequest(new { message = $"A package with title '{request.Title}' already exists." });
            //    }

            //    package.MenuCategoryId = request.CategoryId;
            //    package.Title = request.Title.Trim();
            //    package.Description = request.Description.Trim();
            //    package.CardSummary = request.CardSummary.Trim();
            //    package.Badge = request.Badge.Trim();
            //    package.Notice = request.Notice.Trim();
            //    package.ServesLabel = request.ServesLabel.Trim();
            //    package.InclusionText = request.InclusionText.Trim();
            //    package.ImageUrl = request.ImageUrl.Trim();
            //    package.Rating = request.Rating;
            //    package.ReviewCount = request.ReviewCount;
            //    package.IsAvailable = request.IsAvailable;
            //    package.IsCustomizable = request.IsCustomizable;
            //    package.UpdatedAtUtc = DateTime.UtcNow;

            //    // Sizes: update existing or add new, but do not delete omitted ones
            //    foreach (var sizeRequest in request.Sizes)
            //    {
            //        if (string.IsNullOrWhiteSpace(sizeRequest.Label))
            //        {
            //            return Results.BadRequest(new { message = "Each package size must have a label." });
            //        }

            //        if (sizeRequest.PaxCount <= 0)
            //        {
            //            return Results.BadRequest(new { message = $"Package size '{sizeRequest.Label}' must have a pax count greater than zero." });
            //        }

            //        if (sizeRequest.Price < 0)
            //        {
            //            return Results.BadRequest(new { message = $"Package size '{sizeRequest.Label}' cannot have a negative price." });
            //        }

            //        var existingSize = sizeRequest.Id.HasValue
            //            ? package.Sizes.FirstOrDefault(x => x.Id == sizeRequest.Id.Value)
            //            : null;

            //        if (existingSize is null)
            //        {
            //            package.Sizes.Add(new PackageSize
            //            {
            //                Label = sizeRequest.Label.Trim(),
            //                Subtitle = sizeRequest.Subtitle.Trim(),
            //                PaxCount = sizeRequest.PaxCount,
            //                Price = sizeRequest.Price
            //            });
            //        }
            //        else
            //        {
            //            existingSize.Label = sizeRequest.Label.Trim();
            //            existingSize.Subtitle = sizeRequest.Subtitle.Trim();
            //            existingSize.PaxCount = sizeRequest.PaxCount;
            //            existingSize.Price = sizeRequest.Price;
            //        }
            //    }

            //    // Addons: update existing or add new, but do not delete omitted ones
            //    foreach (var addonRequest in request.Addons)
            //    {
            //        if (string.IsNullOrWhiteSpace(addonRequest.Name))
            //        {
            //            return Results.BadRequest(new { message = "Each add-on must have a name." });
            //        }

            //        if (addonRequest.Price < 0)
            //        {
            //            return Results.BadRequest(new { message = $"Add-on '{addonRequest.Name}' cannot have a negative price." });
            //        }

            //        var existingAddon = addonRequest.Id.HasValue
            //            ? package.Addons.FirstOrDefault(x => x.Id == addonRequest.Id.Value)
            //            : null;

            //        if (existingAddon is null)
            //        {
            //            package.Addons.Add(new PackageAddon
            //            {
            //                Name = addonRequest.Name.Trim(),
            //                Description = addonRequest.Description.Trim(),
            //                Price = addonRequest.Price,
            //                IsAvailable = addonRequest.IsAvailable
            //            });
            //        }
            //        else
            //        {
            //            existingAddon.Name = addonRequest.Name.Trim();
            //            existingAddon.Description = addonRequest.Description.Trim();
            //            existingAddon.Price = addonRequest.Price;
            //            existingAddon.IsAvailable = addonRequest.IsAvailable;
            //        }
            //    }

            //    // Selection rules and options: update existing or add new, but do not delete omitted ones
            //    foreach (var ruleRequest in request.SelectionRules)
            //    {
            //        if (!TryResolvePackageSelectionType(ruleRequest.SelectionType, out var selectionType))
            //        {
            //            return Results.BadRequest(new
            //            {
            //                message = $"Invalid selection type '{ruleRequest.SelectionType}' for rule '{ruleRequest.Title}'."
            //            });
            //        }

            //        if (!TryResolveMealType(ruleRequest.AllowedMealType, out var allowedMealType))
            //        {
            //            return Results.BadRequest(new
            //            {
            //                message = $"Invalid meal type '{ruleRequest.AllowedMealType}' for rule '{ruleRequest.Title}'."
            //            });
            //        }

            //        if (string.IsNullOrWhiteSpace(ruleRequest.Title))
            //        {
            //            return Results.BadRequest(new { message = "Each selection rule must have a title." });
            //        }

            //        if (ruleRequest.MaxSelections < ruleRequest.MinSelections)
            //        {
            //            return Results.BadRequest(new
            //            {
            //                message = $"Rule '{ruleRequest.Title}' has MaxSelections lower than MinSelections."
            //            });
            //        }

            //        PackageSelectionRule rule;

            //        if (ruleRequest.Id.HasValue)
            //        {
            //            rule = package.SelectionRules.FirstOrDefault(x => x.Id == ruleRequest.Id.Value)
            //                ?? new PackageSelectionRule();

            //            if (!package.SelectionRules.Contains(rule))
            //            {
            //                package.SelectionRules.Add(rule);
            //            }
            //        }
            //        else
            //        {
            //            rule = new PackageSelectionRule();
            //            package.SelectionRules.Add(rule);
            //        }

            //        rule.Title = ruleRequest.Title.Trim();
            //        rule.Description = ruleRequest.Description.Trim();
            //        rule.SelectionType = selectionType;
            //        rule.AllowedMealType = allowedMealType;
            //        rule.MinSelections = ruleRequest.MinSelections;
            //        rule.MaxSelections = ruleRequest.MaxSelections;
            //        rule.IsRequired = ruleRequest.IsRequired;
            //        rule.DisplayOrder = ruleRequest.DisplayOrder;

            //        foreach (var optionRequest in ruleRequest.Options)
            //        {
            //            var meal = await db.Meals.FirstOrDefaultAsync(x => x.Id == optionRequest.MealId);

            //            if (meal is null)
            //            {
            //                return Results.BadRequest(new
            //                {
            //                    message = $"Meal with id {optionRequest.MealId} was not found for rule '{ruleRequest.Title}'."
            //                });
            //            }

            //            if (meal.MealType != allowedMealType)
            //            {
            //                return Results.BadRequest(new
            //                {
            //                    message = $"Meal '{meal.Name}' does not match the allowed meal type '{allowedMealType}' for rule '{ruleRequest.Title}'."
            //                });
            //            }

            //            PackageSelectionOption option;

            //            if (optionRequest.Id.HasValue)
            //            {
            //                option = rule.Options.FirstOrDefault(x => x.Id == optionRequest.Id.Value)
            //                    ?? new PackageSelectionOption();

            //                if (!rule.Options.Contains(option))
            //                {
            //                    rule.Options.Add(option);
            //                }
            //            }
            //            else
            //            {
            //                option = new PackageSelectionOption();
            //                rule.Options.Add(option);
            //            }

            //            option.MealId = optionRequest.MealId;
            //            option.AdditionalPrice = optionRequest.AdditionalPrice;
            //            option.IsDefault = optionRequest.IsDefault;
            //        }
            //    }

            //    await db.SaveChangesAsync();

            //    var updated = await db.Packages
            //        .AsNoTracking()
            //        .Where(x => x.Id == package.Id)
            //        .Include(x => x.MenuCategory)
            //        .Include(x => x.Sizes)
            //        .Include(x => x.Addons)
            //        .Include(x => x.SelectionRules)
            //            .ThenInclude(x => x.Options)
            //                .ThenInclude(x => x.Meal)
            //        .Select(x => new AdminPackageDetailsDto
            //        {
            //            Id = x.Id,
            //            CategoryId = x.MenuCategoryId,
            //            CategoryName = x.MenuCategory.Name,

            //            Title = x.Title,
            //            Description = x.Description,
            //            CardSummary = x.CardSummary,
            //            Badge = x.Badge,
            //            Notice = x.Notice,
            //            ServesLabel = x.ServesLabel,
            //            InclusionText = x.InclusionText,
            //            ImageUrl = x.ImageUrl,

            //            Rating = x.Rating,
            //            ReviewCount = x.ReviewCount,

            //            IsAvailable = x.IsAvailable,
            //            IsCustomizable = x.IsCustomizable,

            //            Sizes = x.Sizes
            //                .OrderBy(s => s.PaxCount)
            //                .Select(s => new AdminPackageSizeDto
            //                {
            //                    Id = s.Id,
            //                    Label = s.Label,
            //                    Subtitle = s.Subtitle,
            //                    PaxCount = s.PaxCount,
            //                    Price = s.Price
            //                })
            //                .ToList(),

            //            Addons = x.Addons
            //                .OrderBy(a => a.Name)
            //                .Select(a => new AdminPackageAddonDto
            //                {
            //                    Id = a.Id,
            //                    Name = a.Name,
            //                    Description = a.Description,
            //                    Price = a.Price,
            //                    IsAvailable = a.IsAvailable
            //                })
            //                .ToList(),

            //            SelectionRules = x.SelectionRules
            //                .OrderBy(r => r.DisplayOrder)
            //                .Select(r => new AdminPackageSelectionRuleDto
            //                {
            //                    Id = r.Id,
            //                    Title = r.Title,
            //                    Description = r.Description,
            //                    SelectionType = r.SelectionType.ToString(),
            //                    AllowedMealType = r.AllowedMealType.ToString(),
            //                    MinSelections = r.MinSelections,
            //                    MaxSelections = r.MaxSelections,
            //                    IsRequired = r.IsRequired,
            //                    DisplayOrder = r.DisplayOrder,
            //                    Options = r.Options
            //                        .OrderBy(o => o.Meal.Name)
            //                        .Select(o => new AdminPackageSelectionOptionDto
            //                        {
            //                            Id = o.Id,
            //                            MealId = o.MealId,
            //                            MealName = o.Meal.Name,
            //                            MealType = o.Meal.MealType.ToString(),
            //                            AdditionalPrice = o.AdditionalPrice,
            //                            IsDefault = o.IsDefault
            //                        })
            //                        .ToList()
            //                })
            //                .ToList()
            //        })
            //        .FirstOrDefaultAsync();

            //    return Results.Ok(updated);
            //});

            ////GET list of meals for admin meal management page
            //app.MapGet("/api/admin/meals", async (CmsDbContext db) =>
            //{
            //    var meals = await db.Meals
            //        .AsNoTracking()
            //        .Include(x => x.MenuCategory)
            //        .OrderBy(x => x.Name)
            //        .Select(x => new AdminMealListItemDto
            //        {
            //            Id = x.Id,
            //            CategoryId = x.MenuCategoryId,
            //            CategoryName = x.MenuCategory.Name,
            //            Name = x.Name,
            //            Description = x.Description,
            //            MealType = x.MealType.ToString(),
            //            BasePrice = x.BasePrice,
            //            StockQuantity = x.StockQuantity,
            //            MinOrderQuantity = x.MinOrderQuantity,
            //            ImageUrl = x.ImageUrl,
            //            IsAvailable = x.IsAvailable,
            //            IsViandOption = x.IsViandOption
            //        })
            //        .ToListAsync();

            //    return Results.Ok(meals);
            //});

            ////GET meal details by id for admin meal management page
            //app.MapGet("/api/admin/meals/{id:int}", async (int id, CmsDbContext db) =>
            //{
            //    var meal = await db.Meals
            //        .AsNoTracking()
            //        .Where(x => x.Id == id)
            //        .Include(x => x.MenuCategory)
            //        .Select(x => new AdminMealDetailsDto
            //        {
            //            Id = x.Id,
            //            CategoryId = x.MenuCategoryId,
            //            CategoryName = x.MenuCategory.Name,
            //            Name = x.Name,
            //            Description = x.Description,
            //            MealType = x.MealType.ToString(),
            //            BasePrice = x.BasePrice,
            //            StockQuantity = x.StockQuantity,
            //            MinOrderQuantity = x.MinOrderQuantity,
            //            ImageUrl = x.ImageUrl,
            //            IsAvailable = x.IsAvailable,
            //            IsViandOption = x.IsViandOption
            //        })
            //        .FirstOrDefaultAsync();

            //    return meal is not null
            //        ? Results.Ok(meal)
            //        : Results.NotFound(new { message = $"Meal with id {id} was not found." });
            //});

            ////POST create new meal for admin meal management page
            //app.MapPost("/api/admin/meals", async (SaveMealRequest request, CmsDbContext db) =>
            //{
            //    if (request.CategoryId <= 0)
            //    {
            //        return Results.BadRequest(new { message = "CategoryId is required." });
            //    }

            //    if (string.IsNullOrWhiteSpace(request.Name))
            //    {
            //        return Results.BadRequest(new { message = "Meal name is required." });
            //    }

            //    if (!TryResolveMealType(request.MealType, out var mealType))
            //    {
            //        return Results.BadRequest(new { message = $"Invalid meal type '{request.MealType}'." });
            //    }

            //    if (request.BasePrice < 0)
            //    {
            //        return Results.BadRequest(new { message = "BasePrice cannot be negative." });
            //    }

            //    if (request.StockQuantity < 0)
            //    {
            //        return Results.BadRequest(new { message = "StockQuantity cannot be negative." });
            //    }

            //    if (request.MinOrderQuantity <= 0)
            //    {
            //        return Results.BadRequest(new { message = "MinOrderQuantity must be greater than zero." });
            //    }

            //    var category = await db.MenuCategories.FirstOrDefaultAsync(x => x.Id == request.CategoryId);
            //    if (category is null)
            //    {
            //        return Results.BadRequest(new { message = $"Category with id {request.CategoryId} was not found." });
            //    }

            //    var duplicateExists = await db.Meals.AnyAsync(x => x.Name == request.Name);
            //    if (duplicateExists)
            //    {
            //        return Results.BadRequest(new { message = $"A meal with name '{request.Name}' already exists." });
            //    }

            //    var meal = new Meal
            //    {
            //        MenuCategoryId = request.CategoryId,
            //        Name = request.Name.Trim(),
            //        Description = request.Description.Trim(),
            //        MealType = mealType,
            //        BasePrice = request.BasePrice,
            //        StockQuantity = request.StockQuantity,
            //        MinOrderQuantity = request.MinOrderQuantity,
            //        ImageUrl = request.ImageUrl.Trim(),
            //        IsAvailable = request.IsAvailable,
            //        IsViandOption = request.IsViandOption
            //    };

            //    db.Meals.Add(meal);
            //    await db.SaveChangesAsync();

            //    var created = await db.Meals
            //        .AsNoTracking()
            //        .Where(x => x.Id == meal.Id)
            //        .Include(x => x.MenuCategory)
            //        .Select(x => new AdminMealDetailsDto
            //        {
            //            Id = x.Id,
            //            CategoryId = x.MenuCategoryId,
            //            CategoryName = x.MenuCategory.Name,
            //            Name = x.Name,
            //            Description = x.Description,
            //            MealType = x.MealType.ToString(),
            //            BasePrice = x.BasePrice,
            //            StockQuantity = x.StockQuantity,
            //            MinOrderQuantity = x.MinOrderQuantity,
            //            ImageUrl = x.ImageUrl,
            //            IsAvailable = x.IsAvailable,
            //            IsViandOption = x.IsViandOption
            //        })
            //        .FirstOrDefaultAsync();

            //    return Results.Created($"/api/admin/meals/{meal.Id}", created);
            //});


            ////PUT update existing meal by id for admin meal management page
            //app.MapPut("/api/admin/meals/{id:int}", async (int id, SaveMealRequest request, CmsDbContext db) =>
            //{
            //    if (request.CategoryId <= 0)
            //    {
            //        return Results.BadRequest(new { message = "CategoryId is required." });
            //    }

            //    if (string.IsNullOrWhiteSpace(request.Name))
            //    {
            //        return Results.BadRequest(new { message = "Meal name is required." });
            //    }

            //    if (!TryResolveMealType(request.MealType, out var mealType))
            //    {
            //        return Results.BadRequest(new { message = $"Invalid meal type '{request.MealType}'." });
            //    }

            //    if (request.BasePrice < 0)
            //    {
            //        return Results.BadRequest(new { message = "BasePrice cannot be negative." });
            //    }

            //    if (request.StockQuantity < 0)
            //    {
            //        return Results.BadRequest(new { message = "StockQuantity cannot be negative." });
            //    }

            //    if (request.MinOrderQuantity <= 0)
            //    {
            //        return Results.BadRequest(new { message = "MinOrderQuantity must be greater than zero." });
            //    }

            //    var meal = await db.Meals.FirstOrDefaultAsync(x => x.Id == id);
            //    if (meal is null)
            //    {
            //        return Results.NotFound(new { message = $"Meal with id {id} was not found." });
            //    }

            //    var category = await db.MenuCategories.FirstOrDefaultAsync(x => x.Id == request.CategoryId);
            //    if (category is null)
            //    {
            //        return Results.BadRequest(new { message = $"Category with id {request.CategoryId} was not found." });
            //    }

            //    var duplicateExists = await db.Meals.AnyAsync(x => x.Id != id && x.Name == request.Name);
            //    if (duplicateExists)
            //    {
            //        return Results.BadRequest(new { message = $"A meal with name '{request.Name}' already exists." });
            //    }

            //    meal.MenuCategoryId = request.CategoryId;
            //    meal.Name = request.Name.Trim();
            //    meal.Description = request.Description.Trim();
            //    meal.MealType = mealType;
            //    meal.BasePrice = request.BasePrice;
            //    meal.StockQuantity = request.StockQuantity;
            //    meal.MinOrderQuantity = request.MinOrderQuantity;
            //    meal.ImageUrl = request.ImageUrl.Trim();
            //    meal.IsAvailable = request.IsAvailable;
            //    meal.IsViandOption = request.IsViandOption;
            //    meal.UpdatedAtUtc = DateTime.UtcNow;

            //    await db.SaveChangesAsync();

            //    var updated = await db.Meals
            //        .AsNoTracking()
            //        .Where(x => x.Id == meal.Id)
            //        .Include(x => x.MenuCategory)
            //        .Select(x => new AdminMealDetailsDto
            //        {
            //            Id = x.Id,
            //            CategoryId = x.MenuCategoryId,
            //            CategoryName = x.MenuCategory.Name,
            //            Name = x.Name,
            //            Description = x.Description,
            //            MealType = x.MealType.ToString(),
            //            BasePrice = x.BasePrice,
            //            StockQuantity = x.StockQuantity,
            //            MinOrderQuantity = x.MinOrderQuantity,
            //            ImageUrl = x.ImageUrl,
            //            IsAvailable = x.IsAvailable,
            //            IsViandOption = x.IsViandOption
            //        })
            //        .FirstOrDefaultAsync();

            //    return Results.Ok(updated);
            //});

            ////DELETE archive meal by id for admin meal management page (set IsAvailable to false instead of actually deleting the record)
            //app.MapDelete("/api/admin/meals/{id:int}", async (int id, CmsDbContext db) =>
            //{
            //    var meal = await db.Meals.FirstOrDefaultAsync(x => x.Id == id);

            //    if (meal is null)
            //    {
            //        return Results.NotFound(new { message = $"Meal with id {id} was not found." });
            //    }

            //    meal.IsAvailable = false;
            //    meal.UpdatedAtUtc = DateTime.UtcNow;

            //    await db.SaveChangesAsync();

            //    var response = new DeleteMealResponse
            //    {
            //        Id = meal.Id,
            //        Name = meal.Name,
            //        IsAvailable = meal.IsAvailable,
            //        Message = "Meal was archived successfully."
            //    };

            //    return Results.Ok(response);
            //});


            app.MapControllers();
            await app.RunAsync();
        }

        private static bool TryResolvePaymentMethod(string rawValue, out PaymentMethod paymentMethod)
        {
            paymentMethod = PaymentMethod.Cash;

            var value = rawValue?.Trim().ToLowerInvariant() ?? string.Empty;

            return value switch
            {
                "cash" => SetPaymentMethod(PaymentMethod.Cash, out paymentMethod),
                "cash on delivery" => SetPaymentMethod(PaymentMethod.Cash, out paymentMethod),
                "cod" => SetPaymentMethod(PaymentMethod.Cash, out paymentMethod),
                "gcash" => SetPaymentMethod(PaymentMethod.GCash, out paymentMethod),
                "maya" => SetPaymentMethod(PaymentMethod.Maya, out paymentMethod),
                "credit card" => SetPaymentMethod(PaymentMethod.Card, out paymentMethod),
                "card" => SetPaymentMethod(PaymentMethod.Card, out paymentMethod),
                "bank" => SetPaymentMethod(PaymentMethod.BankTransfer, out paymentMethod),
                "bank transfer" => SetPaymentMethod(PaymentMethod.BankTransfer, out paymentMethod),
                _ => false
            };
        }

        private static bool SetPaymentMethod(PaymentMethod value, out PaymentMethod paymentMethod)
        {
            paymentMethod = value;
            return true;
        }

        private static string GenerateOrderNumber()
        {
            return $"PS-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        private static DateTime BuildEstimatedDeliveryUtc(DateTime deliveryDate, string timeSlot)
        {
            if (DateTime.TryParse($"{deliveryDate:yyyy-MM-dd} {timeSlot}", out var localEstimate))
            {
                return localEstimate.ToUniversalTime().AddMinutes(45);
            }

            return deliveryDate.Date.AddHours(12).ToUniversalTime();
        }

        private static List<TrackingStepDto> BuildTrackingSteps(Order order)
        {
            var estimatedUtc = BuildEstimatedDeliveryUtc(order.DeliveryDate, order.DeliveryTimeSlot);

            return new List<TrackingStepDto>
    {
        new()
        {
            Title = "Order Confirmed",
            Description = "We have received your order.",
            TimestampUtc = order.OrderedAtUtc,
            State = ResolveTrackingStepState(order.Status, OrderStatus.Confirmed)
        },
        new()
        {
            Title = "Preparing Food",
            Description = "Our kitchen is preparing your selected package.",
            TimestampUtc = (int)order.Status >= (int)OrderStatus.Preparing
                ? order.OrderedAtUtc.AddMinutes(15)
                : null,
            State = ResolveTrackingStepState(order.Status, OrderStatus.Preparing)
        },
        new()
        {
            Title = "Out for Delivery",
            Description = "Your order is on the way.",
            TimestampUtc = (int)order.Status >= (int)OrderStatus.OutForDelivery
                ? order.OrderedAtUtc.AddMinutes(45)
                : null,
            State = ResolveTrackingStepState(order.Status, OrderStatus.OutForDelivery)
        },
        new()
        {
            Title = "Delivered",
            Description = "Your order has arrived.",
            TimestampUtc = order.Status == OrderStatus.Delivered
                ? estimatedUtc
                : null,
            State = ResolveTrackingStepState(order.Status, OrderStatus.Delivered)
        }
    };
        }

        private static TrackingStepState ResolveTrackingStepState(OrderStatus currentStatus, OrderStatus stepStatus)
        {
            if (currentStatus == stepStatus)
                return TrackingStepState.Active;

            if ((int)currentStatus > (int)stepStatus && currentStatus != OrderStatus.Cancelled)
                return TrackingStepState.Done;

            return TrackingStepState.Pending;
        }
        private static bool TryResolveAdminOrderStatus(string rawValue, out OrderStatus status)
        {
            status = OrderStatus.Pending;

            var value = rawValue?.Trim().ToLowerInvariant() ?? string.Empty;

            return value switch
            {
                "confirmed" => SetOrderStatus(OrderStatus.Confirmed, out status),
                "preparing" => SetOrderStatus(OrderStatus.Preparing, out status),
                "outfordelivery" => SetOrderStatus(OrderStatus.OutForDelivery, out status),
                "out for delivery" => SetOrderStatus(OrderStatus.OutForDelivery, out status),
                "delivered" => SetOrderStatus(OrderStatus.Delivered, out status),
                "cancelled" => SetOrderStatus(OrderStatus.Cancelled, out status),
                "canceled" => SetOrderStatus(OrderStatus.Cancelled, out status),
                _ => false
            };
        }

        private static bool SetOrderStatus(OrderStatus value, out OrderStatus status)
        {
            status = value;
            return true;
        }

        private static bool TryResolvePackageSelectionType(string rawValue, out PackageSelectionType selectionType)
        {
            selectionType = PackageSelectionType.Fixed;

            var value = rawValue?.Trim().ToLowerInvariant() ?? string.Empty;

            return value switch
            {
                "fixed" => SetPackageSelectionType(PackageSelectionType.Fixed, out selectionType),
                "chooseone" => SetPackageSelectionType(PackageSelectionType.ChooseOne, out selectionType),
                "choose one" => SetPackageSelectionType(PackageSelectionType.ChooseOne, out selectionType),
                "choosemany" => SetPackageSelectionType(PackageSelectionType.ChooseMany, out selectionType),
                "choose many" => SetPackageSelectionType(PackageSelectionType.ChooseMany, out selectionType),
                _ => false
            };
        }

        private static bool SetPackageSelectionType(PackageSelectionType value, out PackageSelectionType selectionType)
        {
            selectionType = value;
            return true;
        }

        private static bool TryResolveMealType(string rawValue, out MealType mealType)
        {
            mealType = MealType.MainDish;

            var value = rawValue?.Trim().ToLowerInvariant() ?? string.Empty;

            return value switch
            {
                "maindish" => SetMealType(MealType.MainDish, out mealType),
                "main dish" => SetMealType(MealType.MainDish, out mealType),
                "viand" => SetMealType(MealType.Viand, out mealType),
                "sidedish" => SetMealType(MealType.SideDish, out mealType),
                "side dish" => SetMealType(MealType.SideDish, out mealType),
                "dessert" => SetMealType(MealType.Dessert, out mealType),
                "kakanin" => SetMealType(MealType.Kakanin, out mealType),
                "beverage" => SetMealType(MealType.Beverage, out mealType),
                _ => false
            };
        }

        private static bool SetMealType(MealType value, out MealType mealType)
        {
            mealType = value;
            return true;
        }

        private static bool TryResolveOrderItemType(OrderItemRequestDto item, out OrderItemType itemType)
        {
            itemType = OrderItemType.Package;

            var raw = item.ItemType?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(raw))
            {
                if (item.MealId.HasValue)
                {
                    itemType = OrderItemType.Meal;
                    return true;
                }

                if (item.PackageId.HasValue)
                {
                    itemType = OrderItemType.Package;
                    return true;
                }

                return false;
            }

            return raw switch
            {
                "package" => SetOrderItemType(OrderItemType.Package, out itemType),
                "meal" => SetOrderItemType(OrderItemType.Meal, out itemType),
                _ => false
            };
        }

        private static bool SetOrderItemType(OrderItemType value, out OrderItemType itemType)
        {
            itemType = value;
            return true;
        }

        private static AuthResponse CreateCustomerAuthResponse(
            AppUser user,
            CustomerProfile profile,
            JwtOptions jwtOptions)
        {
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(jwtOptions.ExpiryMinutes);

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("customer_profile_id", profile.Id.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponse
            {
                Token = tokenValue,
                ExpiresAtUtc = expiresAtUtc,
                AppUserId = user.Id,
                CustomerProfileId = profile.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Role = user.Role.ToString()
            };
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidUsername(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            return Regex.IsMatch(userName, @"^[a-zA-Z0-9._]{4,20}$");
        }

        private static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return Regex.IsMatch(password,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$");
        }

        private static string GenerateVerificationCode()
        {
            return Random.Shared.Next(100000, 999999).ToString();
        }
    }

}
