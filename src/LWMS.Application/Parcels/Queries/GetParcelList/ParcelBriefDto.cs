using LWMS.Domain.Enums;

namespace LWMS.Application.Parcels.Queries.GetParcelList;

public class ParcelBriefDto
{
    public Guid Id { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public ParcelStatus Status { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PaginatedList<T>
{
    public List<T> Items { get; set; } = new();
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }
}
