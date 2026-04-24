using MediatR;
using LWMS.Domain.Entities;
using LWMS.Application.Common.Interfaces;

namespace LWMS.Application.Cod.Commands.Settle;

public record CodSettlementCommand : IRequest<Guid>
{
    public Guid MerchantId { get; init; }
    public List<Guid> CodRecordIds { get; init; } = new();
    public Guid AdminId { get; init; } // Người thực hiện quyết toán
}

public class CodSettlementCommandHandler : IRequestHandler<CodSettlementCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CodSettlementCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CodSettlementCommand request, CancellationToken cancellationToken)
    {
        // 1. Kiểm tra Merchant
        var merchant = await _unitOfWork.Merchants.GetByIdAsync(request.MerchantId);
        if (merchant == null) throw new KeyNotFoundException("Không tìm thấy Merchant.");

        // 2. Tạo phiên quyết toán (Settlement)
        var settlement = new CodSettlement
        {
            MerchantId = request.MerchantId,
            ShipperId = request.AdminId,
            Status = "COMPLETED",
            CreatedDate = DateTime.UtcNow,
            SettledDate = DateTime.UtcNow,
            TotalCollected = 0,
            TotalSubmitted = 0,
            TotalSettled = 0
        };

        decimal totalAmount = 0;

        foreach (var codId in request.CodRecordIds)
        {
            // EF Core tự handle RowVersion (Optimistic Concurrency)
            var codRecord = await _unitOfWork.CodRecords.GetByIdAsync(codId);
            
            if (codRecord == null) continue;
            
            // Chống Double COD
            if (codRecord.SettledAt != null || codRecord.Status == "SETTLED") 
                throw new InvalidOperationException($"Đơn hàng {codId} đã được quyết toán trước đó.");

            // Thêm item vào settlement
            var item = new CodSettlementItem
            {
                ParcelId = codRecord.ParcelId,
                Amount = codRecord.Amount,
                SettlementId = settlement.Id // Sẽ được EF gán khi save
            };
            settlement.Items.Add(item);

            // Cập nhật trạng thái CodRecord
            codRecord.Status = "SETTLED";
            codRecord.SettledAt = DateTime.UtcNow;
            
            totalAmount += codRecord.Amount;
        }

        if (totalAmount == 0) throw new InvalidOperationException("Không có đơn hàng nào hợp lệ để quyết toán.");

        settlement.TotalCollected = totalAmount;
        settlement.TotalSettled = totalAmount;

        await _unitOfWork.CodSettlements.AddAsync(settlement);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return settlement.Id; 
    }
}
