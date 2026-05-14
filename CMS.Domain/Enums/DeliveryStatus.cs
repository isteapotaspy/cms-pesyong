
namespace CMS.Domain.Enums;

public enum DeliveryStatus
{
    Pending = 1,
    Scheduled = 2,
    Preparing = 3,
    OutForDelivery = 4,
    Delivered = 5,
    Failed = 6,
    Cancelled = 7
}