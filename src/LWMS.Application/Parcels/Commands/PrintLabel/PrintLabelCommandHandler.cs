using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LWMS.Application.Parcels.Commands.PrintLabel;
public class PrintLableCommandHandler :IRequestHandler<PrintLabelCommand,bool>
{
    private readonly IUnitOfWork _uwo;
    public PrintLableCommandHandler(IUnitOfWork uwo)
    {
        _uwo = uwo;
    }
    public async Task<bool> Handle(PrintLabelCommand request, CancellationToken cancellationToken)
    {
        var parcel = await _uwo.Parcels.Query().FirstOrDefaultAsync(x=>x.TrackingCode == request.TrackingCode);
        if(parcel == null)
        throw new Exception("Parcel not found");
        //RULE: Only Print Label for parcels that are in "Created" status
        if(parcel.Status !=ParcelStatus.Created)
        throw new Exception("Cannot print label for parcels that are not in 'Created' status");
        //Use Domain Method
        var log = parcel.ChangeStatusWithLog(ParcelStatus.LabelPrinted,Guid.Empty ,"SYSTEM");
        await _uwo.TrackingLogs.AddAsync(log);
        await _uwo.SaveChangesAsync();
        return true;
    }
}
