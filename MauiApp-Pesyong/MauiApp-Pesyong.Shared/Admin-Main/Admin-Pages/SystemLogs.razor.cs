using Microsoft.AspNetCore.Components;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;

namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Pages;

public partial class SystemLogs : ComponentBase
{
    [Inject]
    public AdminDataService DataService { get; set; } = default!;

    protected List<SystemLogVm> logs = new();
    protected string searchText = "";
    protected string filterLevel = "";

    protected override void OnInitialized()
    {
        logs = DataService.GetSystemLogs();
    }

    protected IEnumerable<SystemLogVm> FilteredLogs => logs
        .Where(l =>
            (string.IsNullOrWhiteSpace(searchText) ||
             l.Message.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
             l.Source.Contains(searchText, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(filterLevel) || l.Level == filterLevel))
        .OrderByDescending(l => l.Timestamp);

    protected void Refresh()
    {
        logs = DataService.GetSystemLogs();
    }
}
