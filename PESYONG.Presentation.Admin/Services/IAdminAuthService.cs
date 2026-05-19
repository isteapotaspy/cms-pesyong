using CMS.Contracts.Admin.Auth;

namespace PESYONG.Presentation.Admin.Services;

public interface IAdminAuthService
{
    Task<AdminLoginResponseDto> LoginAsync(AdminLoginRequestDto request);
    Task<AdminLoginResponseDto> VerifyCodeAsync(AdminVerifyCodeRequestDto request);
    Task ResendCodeAsync(AdminResendCodeRequestDto request);
    Task<AdminMeDto?> GetMeAsync();
    Task<bool> TryRestoreSessionAsync();
    Task LogoutAsync();
}