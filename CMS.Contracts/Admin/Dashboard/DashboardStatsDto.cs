using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Dashboard;

public class DashboardStatsDto
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int ConfirmedOrders { get; set; }
    public int DeliveredOrders { get; set; }

    public int TotalCustomers { get; set; }
    public int TotalPackages { get; set; }
    public int TotalMeals { get; set; }

    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
}