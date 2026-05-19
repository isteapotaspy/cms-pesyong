using MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;
using MauiApp_Pesyong.Shared.Services;
using MauiApp_Pesyong.Web.Components;
using MauiApp_Pesyong.Web.Services;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddSingleton<AdminDataService>();

builder.Services.AddMudServices();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5010/";

builder.Services.AddScoped<CustomerDrawerState>();
builder.Services.AddScoped<ShortOrdersVm>();
builder.Services.AddScoped<CustomerSession>();
builder.Services.AddScoped<ICustomerTokenStore, WebCustomerTokenStore>();
builder.Services.AddScoped<CustomerAuthHeaderHandler>();

// Plain named client used by SignalR realtime service for base URL
builder.Services.AddHttpClient("CMSApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// CustomerApiClient now handles bearer tokens itself for protected calls
builder.Services.AddHttpClient<CustomerApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddScoped<ICustomerCatalogService>(sp => sp.GetRequiredService<CustomerApiClient>());
builder.Services.AddScoped<ICustomerOrderService>(sp => sp.GetRequiredService<CustomerApiClient>());

builder.Services.AddHttpClient<ICustomerAuthService, CustomerAuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddScoped<ICustomerOrderRealtimeService, CustomerOrderRealtimeService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(MauiApp_Pesyong.Shared._Imports).Assembly);

app.Run();