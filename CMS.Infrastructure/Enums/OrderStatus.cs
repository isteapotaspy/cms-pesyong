

namespace CMS.Domain.Enums;

public enum OrderStatus
{
    Draft = 1,
    Pending = 2,
    Confirmed = 3,
    Preparing = 4,
    OutForDelivery = 5,
    Delivered = 6,
    Cancelled = 7
}