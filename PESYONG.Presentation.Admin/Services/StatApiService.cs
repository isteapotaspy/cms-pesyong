using System.Net.Http;
using System.Net.Http.Json;
using CMS.Contracts.Admin.Dashboard;
using Microsoft.AspNetCore.SignalR.Client;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.Services;

public sealed class StatApiService : IStatApiService
{
    private readonly HttpClient _httpClient;
    private HubConnection? _hubConnection;

    public event Action<DashboardStatsDto>? StatsUpdated;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public StatApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CMSApi");
    }

    public async Task<DashboardStatsDto?> GetDashboardStatsAsync(
        CancellationToken cancellationToken = default)
    {
        var baseAddress = GetRequiredBaseAddress();

        var statsUri = new Uri(baseAddress, "api/admin/stats");

        return await _httpClient.GetFromJsonAsync<DashboardStatsDto>(
            statsUri,
            cancellationToken);
    }

    public async Task StartStatsStreamAsync(CancellationToken cancellationToken = default)
    {
        if (_hubConnection is { State: HubConnectionState.Connected or HubConnectionState.Connecting })
            return;

        var baseAddress = GetRequiredBaseAddress();
        var hubUri = new Uri(baseAddress, "hubs/admin/stats");

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUri)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<DashboardStatsDto>(DashboardStatsEvents.StatsUpdated, stats =>
        {
            StatsUpdated?.Invoke(stats);
        });

        _hubConnection.Reconnected += async _ =>
        {
            var latestStats = await GetDashboardStatsAsync();

            if (latestStats is not null)
                StatsUpdated?.Invoke(latestStats);
        };

        await _hubConnection.StartAsync(cancellationToken);
    }

    public async Task StopStatsStreamAsync(CancellationToken cancellationToken = default)
    {
        if (_hubConnection is null)
            return;

        await _hubConnection.StopAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
            await _hubConnection.DisposeAsync();
    }

    private Uri GetRequiredBaseAddress()
    {
        if (_httpClient.BaseAddress is null)
        {
            throw new InvalidOperationException(
                "HttpClient.BaseAddress is not configured. Check ApiBaseUrl in appsettings.json and WPF DI registration.");
        }

        if (!_httpClient.BaseAddress.IsAbsoluteUri)
        {
            throw new InvalidOperationException(
                $"HttpClient.BaseAddress must be absolute. Current value: {_httpClient.BaseAddress}");
        }

        return _httpClient.BaseAddress;
    }
}