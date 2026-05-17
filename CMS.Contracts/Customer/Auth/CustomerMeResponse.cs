
namespace CMS.Contracts.Customer.Auth;
public class CustomerMeResponse
{
    public int AppUserId { get; set; }
    public int CustomerProfileId { get; set; }

    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;
}