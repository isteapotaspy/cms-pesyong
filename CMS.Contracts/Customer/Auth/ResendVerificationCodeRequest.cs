namespace CMS.Contracts.Customer.Auth;

public class ResendVerificationCodeRequest
{
    public string Email { get; set; } = string.Empty;
}