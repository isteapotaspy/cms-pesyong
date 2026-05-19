
namespace CMS.Contracts.Customer.Auth;
public class CustomerLoginRequest
{
    public string UserNameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}