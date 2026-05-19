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

// Add device-specific services used by the MauiApp_Pesyong.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddSingleton<AdminDataService>();

builder.Services.AddMudServices();
builder.Services.AddScoped<CustomerDrawerState>();
builder.Services.AddScoped<ShortOrdersVm>();



builder.Services.AddHttpClient("CMSApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5010/");
});

builder.Services.AddScoped<CustomerApiClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return new CustomerApiClient(factory.CreateClient("CMSApi"));
});

builder.Services.AddScoped<ICustomerCatalogService>(sp => sp.GetRequiredService<CustomerApiClient>());
builder.Services.AddScoped<ICustomerOrderService>(sp => sp.GetRequiredService<CustomerApiClient>());

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5010/";

builder.Services.AddScoped<CustomerSession>();
builder.Services.AddScoped<ICustomerTokenStore, WebCustomerTokenStore>();
builder.Services.AddScoped<CustomerAuthHeaderHandler>();

builder.Services.AddHttpClient<ICustomerAuthService, CustomerAuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<CustomerAuthHeaderHandler>();

builder.Services.AddHttpClient<ICustomerCatalogService, CustomerApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<CustomerAuthHeaderHandler>();

builder.Services.AddHttpClient<ICustomerOrderService, CustomerApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<CustomerAuthHeaderHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(MauiApp_Pesyong.Shared._Imports).Assembly);

app.Run();
