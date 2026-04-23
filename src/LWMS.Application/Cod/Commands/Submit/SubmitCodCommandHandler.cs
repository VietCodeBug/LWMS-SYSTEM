using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Cod.Commands.Submit;

public class SubmitCodCommandHandler : IRequestHandler<SubmitCodCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;

    public SubmitCodCommandHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(SubmitCodCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _uow.Parcels.Query().FirstOrDefaultAsync(x => x.TrackingCode == request.TrackingCode, cancellationToken);
        if (parcel == null) throw new LWMS.Application.Common.Exceptions.BusinessException("Parcel not found");

        var codRecord = await _uow.CodRecords.Query().FirstOrDefaultAsync(x => x.ParcelId == parcel.Id && x.Status == "COLLECTED", cancellationToken);
        if (codRecord == null) throw new LWMS.Application.Common.Exceptions.BusinessException("Không tìm thấy bản ghi COD đã thu cho đơn hàng này.");

        // Fraud Check: Kiểm tra số tiền nộp có khớp số tiền cần thu không
        if (request.Amount != codRecord.Amount)
        {
            throw new LWMS.Application.Common.Exceptions.BusinessException($"Số tiền nộp ({request.Amount}) không khớp với số tiền COD đã thu ({codRecord.Amount})!");
        }

        // Security Check: Chỉ người đã thu tiền mới được nộp tiền (Trừ Admin)
        if (_currentUserService.Role != "Admin" && codRecord.CollectedBy != _currentUserService.UserId)
        {
            throw new LWMS.Application.Common.Behaviors.ForbiddenAccessException("Bạn không thể nộp hộ tiền COD của người khác.");
        }

        codRecord.Status = "SUBMITTED";
        codRecord.SubmittedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
