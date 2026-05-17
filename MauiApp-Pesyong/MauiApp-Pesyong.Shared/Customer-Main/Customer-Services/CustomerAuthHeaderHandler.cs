using System.Net.Http.Headers;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public class CustomerAuthHeaderHandler : DelegatingHandler
{
    private readonly ICustomerTokenStore _tokenStore;

    public CustomerAuthHeaderHandler(ICustomerTokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenStore.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}