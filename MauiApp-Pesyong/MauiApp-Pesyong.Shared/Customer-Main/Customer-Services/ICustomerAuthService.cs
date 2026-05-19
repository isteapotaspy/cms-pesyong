using CMS.Contracts.Customer.Auth;
using CMS.Contracts.Customer.Profile;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public interface ICustomerAuthService
{
    Task<RegisterResponse> RegisterAsync(CustomerRegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default);
    Task<string> ResendVerificationCodeAsync(ResendVerificationCodeRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(CustomerLoginRequest request, CancellationToken cancellationToken = default);
    Task<CustomerMeResponse?> GetMeAsync(CancellationToken cancellationToken = default);
    Task<CustomerMeResponse> UpdateProfileAsync(UpdateCustomerProfileRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CustomerAddressDto>> GetSavedAddressesAsync(CancellationToken cancellationToken = default);
    Task<CustomerAddressDto> CreateAddressAsync(SaveCustomerAddressRequest request, CancellationToken cancellationToken = default);
    Task<CustomerAddressDto> UpdateAddressAsync(int addressId, SaveCustomerAddressRequest request, CancellationToken cancellationToken = default);
    Task SetDefaultAddressAsync(int addressId, CancellationToken cancellationToken = default);
    Task DeleteAddressAsync(int addressId, CancellationToken cancellationToken = default);

    Task<bool> TryRestoreSessionAsync(CancellationToken cancellationToken = default);
    Task LogoutAsync();
}