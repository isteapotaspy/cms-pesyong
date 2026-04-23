using Microsoft.AspNetCore.Components;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;

namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Pages;

public partial class Customers : ComponentBase
{
    [Inject]
    public AdminDataService DataService { get; set; } = default!;

    protected List<CustomerVm> customers = new();
    protected CustomerVm selected = new();
    protected string searchText = "";
    protected bool showConfirm;

    protected override void OnInitialized()
    {
        customers = DataService.GetCustomers();
        selected = customers.Any() ? Clone(customers.First()) : new CustomerVm();
    }

    protected IEnumerable<CustomerVm> FilteredCustomers => customers.Where(c =>
        string.IsNullOrWhiteSpace(searchText) ||
        c.FullName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
        c.Email.Contains(searchText, StringComparison.OrdinalIgnoreCase));

    protected void Select(CustomerVm c) => selected = Clone(c);

    protected void CreateNew() => selected = new CustomerVm
    {
        CustomerId = "CUST-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper(),
        CreatedDate = DateTime.Today
    };

    protected void Save()
    {
        var existing = customers.FirstOrDefault(c => c.CustomerId == selected.CustomerId);
        if (existing is null) customers.Add(Clone(selected));
        else
        {
            existing.FirstName = selected.FirstName;
            existing.LastName = selected.LastName;
            existing.Email = selected.Email;
            existing.Phone = selected.Phone;
            existing.Address = selected.Address;
            existing.Notes = selected.Notes;
            existing.CreatedDate = selected.CreatedDate;
        }
    }

    protected void Delete() => showConfirm = true;

    protected void ConfirmDelete()
    {
        showConfirm = false;
        var existing = customers.FirstOrDefault(c => c.CustomerId == selected.CustomerId);
        if (existing is not null) { customers.Remove(existing); CreateNew(); }
    }

    private static CustomerVm Clone(CustomerVm c) => new()
    {
        CustomerId = c.CustomerId, FirstName = c.FirstName, LastName = c.LastName,
        Email = c.Email, Phone = c.Phone, Address = c.Address, Notes = c.Notes, CreatedDate = c.CreatedDate
    };
}
