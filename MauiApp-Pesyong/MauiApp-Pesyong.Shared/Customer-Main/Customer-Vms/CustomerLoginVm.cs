using CMS.Contracts.Customer.Auth;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class CustomerLoginVm
{
    public string UserNameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public CustomerLoginRequest ToRequest()
    {
        return new CustomerLoginRequest
        {
            UserNameOrEmail = UserNameOrEmail.Trim(),
            Password = Password
        };
    }
}