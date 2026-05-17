using CMS.Contracts.Customer.Auth;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public interface ICustomerAuthService
{
    Task<RegisterResponse> RegisterAsync(CustomerRegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default);
    Task<string> ResendVerificationCodeAsync(ResendVerificationCodeRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(CustomerLoginRequest request, CancellationToken cancellationToken = default);
    Task<CustomerMeResponse?> GetMeAsync(CancellationToken cancellationToken = default);
    Task<bool> TryRestoreSessionAsync(CancellationToken cancellationToken = default);
    Task LogoutAsync();
}