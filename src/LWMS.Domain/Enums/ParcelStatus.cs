namespace LWMS.Domain.Enums
{
    public enum ParcelStatus
    {
        Created,
        Picking,
        Picked,
        Intrasit,
        ArrivedHub,    
        OutForDelivery,
        Delivered,
        FailedDelivery,
        Returning,
        Returned,
        Cancelled
    }
}