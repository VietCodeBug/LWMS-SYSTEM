namespace LWMS.Domain.Enums
{
    public enum ParcelStatus
    {
        Created,
        Picking,
        Picked,
        InTransit,
        ArrivedHub,    
        OutForDelivery,
        Delivered,
        FailedDelivery,
        Returning,
        Returned,
        Cancelled
    }
}