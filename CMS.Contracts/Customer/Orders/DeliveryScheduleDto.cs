using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Orders;
public class DeliveryScheduleDto
{
    public DateTime DeliveryDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
}