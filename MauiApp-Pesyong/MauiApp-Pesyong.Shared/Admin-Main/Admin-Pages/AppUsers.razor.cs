using Microsoft.AspNetCore.Components;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;

namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Pages;

public partial class AppUsers : ComponentBase
{
    [Inject]
    public AdminDataService DataService { get; set; } = default!;

    protected List<AppUserVm> users = new();
    protected AppUserVm selected = new();
    protected string searchText = "";
    protected string filterRole = "";
    protected bool showConfirm;

    protected override void OnInitialized()
    {
        users = DataService.GetAppUsers();
        selected = users.Any() ? Clone(users.First()) : new AppUserVm();
    }

    protected IEnumerable<AppUserVm> FilteredUsers => users.Where(u =>
        (string.IsNullOrWhiteSpace(searchText) ||
         u.DisplayName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
         u.Username.Contains(searchText, StringComparison.OrdinalIgnoreCase)) &&
        (string.IsNullOrWhiteSpace(filterRole) || u.Role == filterRole));

    protected void Select(AppUserVm u) => selected = Clone(u);

    protected void CreateNew() => selected = new AppUserVm
    {
        UserId = "USR-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper(),
        Role = "Staff", IsActive = true, LastLogin = DateTime.Now
    };

    protected void Save()
    {
        var existing = users.FirstOrDefault(u => u.UserId == selected.UserId);
        if (existing is null) users.Add(Clone(selected));
        else
        {
            existing.Username = selected.Username;
            existing.DisplayName = selected.DisplayName;
            existing.Email = selected.Email;
            existing.Role = selected.Role;
            existing.IsActive = selected.IsActive;
        }
    }

    protected void Delete() => showConfirm = true;

    protected void ConfirmDelete()
    {
        showConfirm = false;
        var existing = users.FirstOrDefault(u => u.UserId == selected.UserId);
        if (existing is not null) { users.Remove(existing); CreateNew(); }
    }

    private static AppUserVm Clone(AppUserVm u) => new()
    {
        UserId = u.UserId, Username = u.Username, DisplayName = u.DisplayName,
        Email = u.Email, Role = u.Role, IsActive = u.IsActive, LastLogin = u.LastLogin
    };
}
