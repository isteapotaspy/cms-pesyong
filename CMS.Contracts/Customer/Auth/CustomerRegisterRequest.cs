
namespace CMS.Contracts.Customer.Auth;
public class CustomerRegisterRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;
}