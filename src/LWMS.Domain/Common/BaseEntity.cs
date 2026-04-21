namespace LWMS.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Soft delete — không xóa vật lý, chỉ đánh dấu.
    /// Global Query Filter sẽ tự động lọc IsDeleted == false.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Concurrency token — EF Core tự quản lý.
    /// Ngăn 2 người cùng sửa 1 record đồng thời.
    /// </summary>
    public byte[]? RowVersion { get; set; }
}