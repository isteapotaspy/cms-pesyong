using CMS.Contracts.Customer.Orders;
using Microsoft.AspNetCore.SignalR.Client;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public class CustomerOrderRealtimeService : ICustomerOrderRealtimeService
{
    private readonly ICustomerTokenStore _tokenStore;
    private readonly IHttpClientFactory _httpClientFactory;
    private HubConnection? _connection;
    private bool _started;

    public event Action<OrderStatusUpdatedEvent>? OrderStatusUpdated;

    public CustomerOrderRealtimeService(
        ICustomerTokenStore tokenStore,
        IHttpClientFactory httpClientFactory)
    {
        _tokenStore = tokenStore;
        _httpClientFactory = httpClientFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_started && _connection is not null)
            return;

        var token = await _tokenStore.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return;

        var httpClient = _httpClientFactory.CreateClient("CMSApi");

        var baseUri = httpClient.BaseAddress
            ?? throw new InvalidOperationException("HttpClient BaseAddress is not configured.");

        var hubUrl = new Uri(baseUri, "hubs/orders");

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = async () =>
                    await _tokenStore.GetTokenAsync();
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.On<OrderStatusUpdatedEvent>("OrderStatusUpdated", evt =>
        {
            OrderStatusUpdated?.Invoke(evt);
        });

        await _connection.StartAsync(cancellationToken);
        _started = true;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_connection is null)
            return;

        await _connection.StopAsync(cancellationToken);
        _started = false;
    }

    public async Task JoinOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        await StartAsync(cancellationToken);

        if (_connection is not null)
            await _connection.InvokeAsync("JoinOrderGroup", orderId, cancellationToken);
    }

    public async Task LeaveOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        if (_connection is not null && _started)
            await _connection.InvokeAsync("LeaveOrderGroup", orderId, cancellationToken);
    }

    public async Task JoinCustomerAsync(int customerProfileId, CancellationToken cancellationToken = default)
    {
        await StartAsync(cancellationToken);

        if (_connection is not null)
            await _connection.InvokeAsync("JoinCustomerGroup", customerProfileId, cancellationToken);
    }

    public async Task LeaveCustomerAsync(int customerProfileId, CancellationToken cancellationToken = default)
    {
        if (_connection is not null && _started)
            await _connection.InvokeAsync("LeaveCustomerGroup", customerProfileId, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}