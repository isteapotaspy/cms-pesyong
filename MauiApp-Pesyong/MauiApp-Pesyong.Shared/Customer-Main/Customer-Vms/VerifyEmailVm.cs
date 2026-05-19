using CMS.Contracts.Customer.Auth;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class VerifyEmailVm
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public VerifyEmailRequest ToVerifyRequest()
    {
        return new VerifyEmailRequest
        {
            Email = Email.Trim(),
            Code = Code.Trim()
        };
    }

    public ResendVerificationCodeRequest ToResendRequest()
    {
        return new ResendVerificationCodeRequest
        {
            Email = Email.Trim()
        };
    }
}