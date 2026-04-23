namespace LWMS.Domain.Enums
{
    public enum ParcelStatus
    {
        Created,
        LabelPrinted,
        Picking,
        Picked,
        InTransit,
        ArrivedHub,
        Sorted,
        InBag,
        OutForDelivery,
        Delivered,
        FailedDelivery,
        Returning,
        Returned,
        Cancelled
    }
}