using CsvHelper;
using CsvHelper.Configuration;
using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Domain.Enums;
using LWMS.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace LWMS.Application.Parcels.Commands.BulkUpload;

public record BulkUploadResponse(int TotalProcessed, int TotalSuccess, List<string> Errors);

public class BulkUploadCommand : IRequest<BulkUploadResponse>
{
    public Stream Stream { get; set; } = Stream.Null;
    public Guid MerchantId { get; set; }
}

public class BulkUploadCommandHandler : IRequestHandler<BulkUploadCommand, BulkUploadResponse>
{
    private readonly IUnitOfWork _uow;

    public BulkUploadCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<BulkUploadResponse> Handle(BulkUploadCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        int processed = 0;
        int success = 0;

        var defaultHub = await _uow.Hubs.Query().FirstOrDefaultAsync(cancellationToken);
        var defaultService = await _uow.ServiceTypes.Query().FirstOrDefaultAsync(cancellationToken);

        if (defaultHub == null || defaultService == null)
        {
            return new BulkUploadResponse(0, 0, new List<string> { "Lỗi cấu hình hệ thống: Thiếu Hub hoặc Service Type mặc định." });
        }

        using var reader = new StreamReader(request.Stream, Encoding.UTF8);
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        };
        
        using var csv = new CsvReader(reader, csvConfig);
        
        while (await csv.ReadAsync())
        {
            processed++;
            try
            {
                var parcel = new Parcel
                {
                    MerchantId = request.MerchantId,
                    TrackingCode = $"B{DateTime.Now:yyMMdd}{Guid.NewGuid().ToString()[..6].ToUpper()}",
                    ServiceTypeId = defaultService.Id,
                    OriginHubId = defaultHub.Id,
                    DestHubId = defaultHub.Id,
                    
                    SenderName = "Merchant Store",
                    SenderPhone = "0123456789",
                    SenderAddress = new Address("Kho Merchant", "Quận 1", "HCM", "VN"),

                    ReceiverName = csv.GetField<string>(0) ?? "Unknown",
                    ReceiverPhone = csv.GetField<string>(1) ?? "000",
                    ReceiverAddress = new Address(csv.GetField<string>(2) ?? "N/A", "N/A", "N/A", "VN"),
                    
                    Weight = csv.GetField<decimal>(3),
                    CodAmount = new Money(csv.GetField<decimal>(4), "VND"),
                    Description = csv.TryGetField<string>(5, out var note) ? note : "Hàng Bulk",
                    Status = ParcelStatus.Created,
                    CreatedAt = DateTime.UtcNow
                };

                await _uow.Parcels.AddAsync(parcel);
                success++;
            }
            catch (Exception ex)
            {
                errors.Add($"Dòng {processed + 1}: {ex.Message}");
            }
        }

        if (success > 0)
            await _uow.SaveChangesAsync(cancellationToken);

        return new BulkUploadResponse(processed, success, errors);
    }
}
