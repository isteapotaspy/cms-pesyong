namespace CMS.Contracts.Customer.Auth;

public class RegisterResponse
{
    public bool RequiresEmailVerification { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}