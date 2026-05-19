using CMS.Contracts.Customer.Auth;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class CustomerRegisterVm
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public bool PasswordsMatch =>
        string.Equals(Password, ConfirmPassword, StringComparison.Ordinal);

    public CustomerRegisterRequest ToRequest()
    {
        return new CustomerRegisterRequest
        {
            FirstName = FirstName.Trim(),
            LastName = LastName.Trim(),
            UserName = UserName.Trim(),
            Email = Email.Trim(),
            MobileNumber = MobileNumber.Trim(),
            Password = Password
        };
    }
}