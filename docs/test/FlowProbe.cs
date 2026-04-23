using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using FluentValidation;
using LWMS.Application.Bags.Commands.AddParcel;
using LWMS.Application.Bags.Commands.AssignShipper;
using LWMS.Application.Bags.Commands.Create;
using LWMS.Application.Bags.Commands.DeliveryFailed;
using LWMS.Application.Bags.Commands.DeliverySuccess;
using LWMS.Application.Bags.Commands.Receive;
using LWMS.Application.Bags.Commands.Seal;
using LWMS.Application.Common.Behaviors;
using LWMS.Application.Common.Interfaces;
using LWMS.Application.Common.Models;
using LWMS.Application.Parcels.Commands.Create;
using LWMS.Application.Parcels.Commands.PrintLabel;
using LWMS.Application.Parcels.Commands.ScanInbound;
using LWMS.Application.Parcels.Commands.Sort;
using LWMS.Domain.Entities;
using LWMS.Domain.Enums;
using LWMS.Domain.ValueObjects;
using LWMS.Infrastructure.Data;
using LWMS.Infrastructure.Data.Interceptors;
using LWMS.Infrastructure.Repositories;
using LWMS.Infrastructure.Services;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public static class Program
{
    public static async Task Main()
    {
        var report = await FlowProbe.RunAsync();

        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Console.WriteLine(json);
    }
}

internal static class FlowProbe
{
    public static async Task<ProbeReport> RunAsync()
    {
        var report = new ProbeReport();

        using var fixture = new ProbeFixture();

        await RunValidationCheckAsync(report, fixture);
        await RunLoggingChecksAsync(report, fixture);
        await RunJwtCheckAsync(report, fixture);
        await RunCreateParcelCheckAsync(report, fixture);
        await RunScanInboundCheckAsync(report, fixture);
        await RunPrintLabelCheckAsync(report, fixture);
        await RunCreateBagCheckAsync(report, fixture);
        await RunAddParcelToBagCheckAsync(report, fixture);
        await RunSealBagCheckAsync(report, fixture);
        await RunReceiveBagCheckAsync(report, fixture);
        await RunAssignShipperCheckAsync(report, fixture);
        await RunDeliveryFailedFirstAttemptCheckAsync(report, fixture);
        await RunDeliveryFailedSecondAttemptCheckAsync(report, fixture);
        await RunDeliveryFailedThirdAttemptCheckAsync(report, fixture);
        await RunDeliverySuccessCheckAsync(report, fixture);

        report.Total = report.Checks.Count;
        report.Passed = report.Checks.Count(x => x.Outcome == "PASS");
        report.Failed = report.Checks.Count(x => x.Outcome == "FAIL");
        return report;
    }

    private static async Task RunValidationCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        var behavior = new ValidationBehavior<CreateParcelCommand, Guid>(
            new IValidator<CreateParcelCommand>[] { new CreateParcelCommandValidator() });

        var invalidCommand = fixture.NewCreateParcelCommand("VAL-001");
        invalidCommand.Weight = 0;

        try
        {
            await behavior.Handle(
                invalidCommand,
                () => Task.FromResult(Guid.NewGuid()),
                CancellationToken.None);

            report.Fail(
                "validation_invalid_weight",
                "Validation should reject Weight <= 0",
                "ValidationBehavior allowed the command to pass.");
        }
        catch (ValidationException ex)
        {
            var hasWeightError = ex.Errors.Any(x => x.PropertyName == nameof(CreateParcelCommand.Weight));
            if (hasWeightError)
            {
                report.Pass(
                    "validation_invalid_weight",
                    "Validation rejected Weight <= 0 as expected.");
            }
            else
            {
                report.Fail(
                    "validation_invalid_weight",
                    "Validation should include a Weight error",
                    "ValidationException was thrown, but Weight was not included.");
            }
        }
    }

    private static Task RunLoggingChecksAsync(ProbeReport report, ProbeFixture fixture)
    {
        var logger = new CaptureLogger<LoggingBehavior<ProbeRequest, string>>();
        var behavior = new LoggingBehavior<ProbeRequest, string>(logger, fixture.CurrentUser);

        var request = new ProbeRequest();

        var successResponse = behavior.Handle(
            request,
            () => Task.FromResult("ok"),
            CancellationToken.None).GetAwaiter().GetResult();

        if (successResponse == "ok"
            && logger.Messages.Any(x => x.Contains("[START]"))
            && logger.Messages.Any(x => x.Contains("[SUCCESS]")))
        {
            report.Pass(
                "logging_success",
                "LoggingBehavior emitted [START] and [SUCCESS].");
        }
        else
        {
            report.Fail(
                "logging_success",
                "LoggingBehavior should emit [START] and [SUCCESS]",
                string.Join(" | ", logger.Messages));
        }

        var errorLogger = new CaptureLogger<LoggingBehavior<ProbeRequest, string>>();
        var errorBehavior = new LoggingBehavior<ProbeRequest, string>(errorLogger, fixture.CurrentUser);

        try
        {
            errorBehavior.Handle(
                request,
                () => Task.FromException<string>(new InvalidOperationException("boom")),
                CancellationToken.None).GetAwaiter().GetResult();

            report.Fail(
                "logging_error",
                "LoggingBehavior should rethrow and emit [ERROR]",
                "The delegate completed successfully.");
        }
        catch (InvalidOperationException)
        {
            if (errorLogger.Messages.Any(x => x.Contains("[ERROR]")))
            {
                report.Pass(
                    "logging_error",
                    "LoggingBehavior emitted [ERROR] and rethrew the exception.");
            }
            else
            {
                report.Fail(
                    "logging_error",
                    "LoggingBehavior should emit [ERROR]",
                    string.Join(" | ", errorLogger.Messages));
            }
        }

        return Task.CompletedTask;
    }

    private static Task RunJwtCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        var jwtService = new JwtService(new JwtSettings
        {
            Issuer = "probe-issuer",
            Audience = "probe-audience",
            SecretKey = "0123456789abcdef0123456789abcdef",
            ExpiryMinutes = 60
        });

        var token = jwtService.GenerateToken(
            fixture.CurrentUser.UserId!.Value,
            "Probe Admin",
            "Admin",
            fixture.MerchantId);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var nameId = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        var role = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        var merchantId = jwt.Claims.FirstOrDefault(x => x.Type == "MerchantId")?.Value;

        if (nameId == fixture.CurrentUser.UserId.Value.ToString()
            && role == "Admin"
            && merchantId == fixture.MerchantId.ToString())
        {
            report.Pass(
                "jwt_claims",
                "JwtService emitted nameid, role, and MerchantId claims.");
        }
        else
        {
            report.Fail(
                "jwt_claims",
                "JWT should contain nameid, role, and MerchantId claims",
                $"nameid={nameId}, role={role}, merchantId={merchantId}");
        }

        return Task.CompletedTask;
    }

    private static async Task RunCreateParcelCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        var handler = new CreateParcelCommandHandler(fixture.UnitOfWork, fixture.CurrentUser);
        var command = fixture.NewCreateParcelCommand("CRT-001");

        var parcelId = await handler.Handle(command, CancellationToken.None);
        var parcel = await fixture.ReloadParcelAsync(parcelId);
        var logs = await fixture.Context.TrackingLogs
            .Where(x => x.ParcelId == parcelId)
            .ToListAsync();

        if (parcel != null
            && parcel.Status == ParcelStatus.Created
            && parcel.DeliveryAttempts == 0
            && parcel.MerchantId == fixture.MerchantId
            && logs.Count == 1
            && logs[0].ToStatus == ParcelStatus.Created)
        {
            report.Pass(
                "create_parcel",
                "CreateParcel created a Created parcel with MerchantId and a tracking log.");
        }
        else
        {
            report.Fail(
                "create_parcel",
                "CreateParcel should create a Created parcel with MerchantId and log",
                $"status={parcel?.Status}, attempts={parcel?.DeliveryAttempts}, logs={logs.Count}");
        }
    }

    private static async Task RunScanInboundCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        var createHandler = new CreateParcelCommandHandler(fixture.UnitOfWork, fixture.CurrentUser);
        var scanHandler = new ScanInboundCommandHandler(fixture.UnitOfWork);

        await createHandler.Handle(fixture.NewCreateParcelCommand("SCN-001"), CancellationToken.None);

        try
        {
            await scanHandler.Handle(new ScanInboundCommand
            {
                TrackingCode = "SCN-001",
                HubId = fixture.OriginHubId
            }, CancellationToken.None);

            report.Pass(
                "scan_inbound_after_create",
                "ScanInbound completed directly after CreateParcel.");
        }
        catch (Exception ex)
        {
            report.Fail(
                "scan_inbound_after_create",
                "Flow expects CreateParcel -> ScanInbound to succeed without hacks",
                ex.Message);
        }
    }

    private static async Task RunPrintLabelCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        var createHandler = new CreateParcelCommandHandler(fixture.UnitOfWork, fixture.CurrentUser);
        var printHandler = new PrintLableCommandHandler(fixture.UnitOfWork);

        await createHandler.Handle(fixture.NewCreateParcelCommand("LBL-001"), CancellationToken.None);

        try
        {
            await printHandler.Handle(new PrintLabelCommand
            {
                TrackingCode = "LBL-001"
            }, CancellationToken.None);

            report.Pass(
                "print_label_after_create",
                "PrintLabel moved Created -> LabelPrinted.");
        }
        catch (Exception ex)
        {
            report.Fail(
                "print_label_after_create",
                "If ScanInbound needs LabelPrinted, PrintLabel must bridge Created -> LabelPrinted",
                ex.Message);
        }
    }

    private static async Task RunCreateBagCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        var handler = new CreateBagCommandHandler(fixture.UnitOfWork);
        var bagId = await handler.Handle(new CreateBagCommand
        {
            FromHubId = fixture.OriginHubId,
            ToHubId = fixture.DestHubId
        }, CancellationToken.None);

        var bag = await fixture.ReloadBagAsync(bagId);

        if (bag != null && bag.Status == BagStatus.Open && bag.ParcelCount == 0)
        {
            report.Pass(
                "create_bag",
                "CreateBag created an Open bag.");
        }
        else
        {
            report.Fail(
                "create_bag",
                "CreateBag should create an Open bag",
                $"status={bag?.Status}, count={bag?.ParcelCount}");
        }
    }

    private static async Task RunAddParcelToBagCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        var bag = await fixture.SeedBagAsync(BagStatus.Open, "BAG-ADD-001");
        await fixture.SeedParcelAsync("ADD-001", ParcelStatus.InTransit, null);

        var handler = new AddParcelToBagCommandHandler(fixture.UnitOfWork);

        try
        {
            await handler.Handle(new AddParcelToBagCommand
            {
                BagId = bag.Id,
                TrackingCode = "ADD-001"
            }, CancellationToken.None);

            report.Pass(
                "add_parcel_to_bag",
                "AddParcelToBag succeeded for an in-transit parcel.");
        }
        catch (Exception ex)
        {
            report.Fail(
                "add_parcel_to_bag",
                "AddParcelToBag should accept a parcel that is ready for bagging",
                ex.Message);
        }
    }

    private static async Task RunSealBagCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        var bag = await fixture.SeedBagAsync(BagStatus.Open, "BAG-SEAL-001", parcelCount: 1);
        var parcel = await fixture.SeedParcelAsync("SEAL-001", ParcelStatus.InTransit, null);
        await fixture.SeedBagItemAsync(bag.Id, parcel.Id);

        var handler = new SealBagCommandHandler(fixture.UnitOfWork);

        try
        {
            await handler.Handle(new SealBagCommand
            {
                BagId = bag.Id
            }, CancellationToken.None);

            report.Pass(
                "seal_bag",
                "SealBag sealed the bag successfully.");
        }
        catch (Exception ex)
        {
            report.Fail(
                "seal_bag",
                "SealBag should succeed when the bag has parcels",
                ex.Message);
        }
    }

    private static async Task RunReceiveBagCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        var bag = await fixture.SeedBagAsync(BagStatus.Sealed, "BAG-RCV-001", parcelCount: 1);
        var parcel = await fixture.SeedParcelAsync("RCV-001", ParcelStatus.InTransit, null);
        await fixture.SeedBagItemAsync(bag.Id, parcel.Id);

        var handler = new ReceiveBagCommandHandler(fixture.UnitOfWork);
        await handler.Handle(new ReceiveBagCommand
        {
            BagId = bag.Id
        }, CancellationToken.None);

        var updatedParcel = await fixture.ReloadParcelAsync(parcel.Id);

        if (updatedParcel != null
            && updatedParcel.Status == ParcelStatus.ArrivedHub
            && updatedParcel.CurrentHubId == fixture.DestHubId)
        {
            report.Pass(
                "receive_bag",
                "ReceiveBag moved the parcel to ArrivedHub and set CurrentHubId.");
        }
        else
        {
            report.Fail(
                "receive_bag",
                "ReceiveBag should set ArrivedHub and CurrentHubId for the destination hub",
                $"status={updatedParcel?.Status}, currentHubId={updatedParcel?.CurrentHubId}");
        }
    }

    private static async Task RunAssignShipperCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        await fixture.SeedParcelAsync("ASN-001", ParcelStatus.ArrivedHub, fixture.DestHubId);

        var handler = new AssignShipperCommandHandler(fixture.UnitOfWork);
        await handler.Handle(new AssignShipperCommand
        {
            TrackingCode = "ASN-001",
            ShipperId = fixture.ShipperId
        }, CancellationToken.None);

        var parcel = await fixture.ReloadParcelAsync("ASN-001");
        var assignment = await fixture.Context.ShipperAssignments
            .FirstOrDefaultAsync(x => x.ParcelId == parcel!.Id);

        if (parcel.Status == ParcelStatus.OutForDelivery && assignment != null)
        {
            report.Pass(
                "assign_shipper",
                "AssignShipper created an assignment and moved the parcel to OutForDelivery.");
        }
        else
        {
            report.Fail(
                "assign_shipper",
                "AssignShipper should create an assignment and move to OutForDelivery",
                $"status={parcel.Status}, assignment={(assignment != null ? "yes" : "no")}");
        }
    }

    private static async Task RunDeliveryFailedFirstAttemptCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        var parcel = await fixture.SeedParcelAsync("FAIL-001", ParcelStatus.OutForDelivery, fixture.DestHubId);

        var handler = new DeliveryFailedCommandHandler(fixture.UnitOfWork);
        await handler.Handle(new DeliveryFailedCommand
        {
            TrackingCode = "FAIL-001",
            Reason = "Recipient absent"
        }, CancellationToken.None);

        var updatedParcel = await fixture.ReloadParcelAsync(parcel.Id);
        var returnOrder = await fixture.Context.ReturnOrders.FirstOrDefaultAsync(x => x.ParcelId == parcel.Id);

        if (updatedParcel != null
            && updatedParcel.Status == ParcelStatus.FailedDelivery
            && updatedParcel.DeliveryAttempts == 1
            && returnOrder == null)
        {
            report.Pass(
                "delivery_failed_first_attempt",
                "First failed delivery increments attempts to 1 and keeps the parcel in FailedDelivery.");
        }
        else
        {
            report.Fail(
                "delivery_failed_first_attempt",
                "First failed delivery should set FailedDelivery and attempts = 1",
                $"status={updatedParcel?.Status}, attempts={updatedParcel?.DeliveryAttempts}, returnOrder={(returnOrder != null ? "yes" : "no")}");
        }
    }

    private static async Task RunDeliveryFailedSecondAttemptCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        await fixture.SeedParcelAsync("FAIL-002", ParcelStatus.OutForDelivery, fixture.DestHubId);

        var handler = new DeliveryFailedCommandHandler(fixture.UnitOfWork);
        await handler.Handle(new DeliveryFailedCommand
        {
            TrackingCode = "FAIL-002",
            Reason = "Recipient absent"
        }, CancellationToken.None);

        try
        {
            await handler.Handle(new DeliveryFailedCommand
            {
                TrackingCode = "FAIL-002",
                Reason = "Recipient absent again"
            }, CancellationToken.None);

            report.Pass(
                "delivery_failed_second_attempt",
                "Second failed delivery stayed in FailedDelivery with attempts = 2.");
        }
        catch (Exception ex)
        {
            report.Fail(
                "delivery_failed_second_attempt",
                "Second failed delivery should be possible without SQL hacks",
                ex.Message);
        }
    }

    private static async Task RunDeliveryFailedThirdAttemptCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        var parcel = await fixture.SeedParcelAsync(
            "FAIL-003",
            ParcelStatus.OutForDelivery,
            fixture.DestHubId,
            deliveryAttempts: 2);

        var handler = new DeliveryFailedCommandHandler(fixture.UnitOfWork);
        await handler.Handle(new DeliveryFailedCommand
        {
            TrackingCode = "FAIL-003",
            Reason = "Third failure"
        }, CancellationToken.None);

        var updatedParcel = await fixture.ReloadParcelAsync(parcel.Id);
        var returnOrder = await fixture.Context.ReturnOrders.FirstOrDefaultAsync(x => x.ParcelId == parcel.Id);

        if (updatedParcel != null
            && updatedParcel.Status == ParcelStatus.Returning
            && updatedParcel.DeliveryAttempts == 3
            && returnOrder != null)
        {
            report.Pass(
                "delivery_failed_third_attempt_seeded",
                "Third failed delivery creates a ReturnOrder when attempts already equal 2.");
        }
        else
        {
            report.Fail(
                "delivery_failed_third_attempt_seeded",
                "Third failed delivery should create a ReturnOrder",
                $"status={updatedParcel?.Status}, attempts={updatedParcel?.DeliveryAttempts}, returnOrder={(returnOrder != null ? "yes" : "no")}");
        }
    }

    private static async Task RunDeliverySuccessCheckAsync(ProbeReport report, ProbeFixture fixture)
    {
        var parcel = await fixture.SeedParcelAsync(
            "SUC-001",
            ParcelStatus.OutForDelivery,
            fixture.DestHubId,
            deliveryAttempts: 2);

        var handler = new DeliverySuccessCommandHandler(fixture.UnitOfWork);
        await handler.Handle(new DeliverySuccessCommand
        {
            TrackingCode = "SUC-001",
            CodAmount = 150000
        }, CancellationToken.None);

        var updatedParcel = await fixture.ReloadParcelAsync(parcel.Id);
        var codRecord = await fixture.Context.CodRecords.FirstOrDefaultAsync(x => x.ParcelId == parcel.Id);
        var logs = await fixture.Context.TrackingLogs
            .Where(x => x.ParcelId == parcel.Id && x.ToStatus == ParcelStatus.Delivered)
            .ToListAsync();

        if (updatedParcel != null
            && updatedParcel.Status == ParcelStatus.Delivered
            && updatedParcel.DeliveryAttempts == 0
            && codRecord != null
            && codRecord.Amount == 150000
            && logs.Count == 1)
        {
            report.Pass(
                "delivery_success",
                "DeliverySuccess delivered the parcel, reset attempts, and created a COD record.");
        }
        else
        {
            report.Fail(
                "delivery_success",
                "DeliverySuccess should deliver the parcel, reset attempts, and create a COD record",
                $"status={updatedParcel?.Status}, attempts={updatedParcel?.DeliveryAttempts}, cod={(codRecord != null ? codRecord.Status : "none")}, logs={logs.Count}");
        }
    }
}

internal sealed class ProbeReport
{
    public int Total { get; set; }
    public int Passed { get; set; }
    public int Failed { get; set; }
    public List<ProbeCheck> Checks { get; } = new();

    public void Pass(string step, string actual)
    {
        Checks.Add(new ProbeCheck
        {
            Step = step,
            Outcome = "PASS",
            Actual = actual
        });
    }

    public void Fail(string step, string expected, string actual)
    {
        Checks.Add(new ProbeCheck
        {
            Step = step,
            Outcome = "FAIL",
            Expected = expected,
            Actual = actual
        });
    }
}

internal sealed class ProbeCheck
{
    public string Step { get; set; } = string.Empty;
    public string Outcome { get; set; } = string.Empty;
    public string Expected { get; set; } = string.Empty;
    public string Actual { get; set; } = string.Empty;
}

internal sealed class ProbeRequest : IRequest<string>
{
}

internal sealed class ProbeFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public ProbeFixture()
    {
        MerchantId = Guid.NewGuid();
        OriginHubId = Guid.NewGuid();
        DestHubId = Guid.NewGuid();
        ServiceTypeId = Guid.NewGuid();
        ShipperId = Guid.NewGuid();

        CurrentUser = new ProbeCurrentUserService
        {
            UserId = Guid.NewGuid(),
            MerchantId = MerchantId,
            Role = "Admin"
        };

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var interceptor = new AuditInterceptor(CurrentUser);

        Context = new AppDbContext(options, CurrentUser, interceptor);
        Context.Database.EnsureCreated();

        UnitOfWork = new UnitOfWork(Context);

        SeedCoreData();
    }

    public AppDbContext Context { get; }
    public IUnitOfWork UnitOfWork { get; }
    public ProbeCurrentUserService CurrentUser { get; }

    public Guid MerchantId { get; }
    public Guid OriginHubId { get; }
    public Guid DestHubId { get; }
    public Guid ServiceTypeId { get; }
    public Guid ShipperId { get; }

    public CreateParcelCommand NewCreateParcelCommand(string trackingCode)
    {
        return new CreateParcelCommand
        {
            SenderName = "Nguyen Van A",
            SenderPhone = "0988888888",
            ReceiverName = "Tran Van B",
            ReceiverPhone = "0999999999",
            Province = "Ha Noi",
            Weight = 2.5m,
            CodAmount = 150000,
            TrackingCode = trackingCode,
            OriginHubId = OriginHubId,
            DestHubId = DestHubId,
            ServiceId = ServiceTypeId,
            Length = 10,
            Width = 10,
            Height = 10
        };
    }

    public async Task<Parcel> SeedParcelAsync(
        string trackingCode,
        ParcelStatus status,
        Guid? currentHubId,
        int deliveryAttempts = 0)
    {
        var parcel = new Parcel
        {
            Id = Guid.NewGuid(),
            TrackingCode = trackingCode,
            SenderName = "Sender",
            SenderPhone = "0988888888",
            ReceiverName = "Receiver",
            ReceiverPhone = "0999999999",
            MerchantId = MerchantId,
            OriginHubId = OriginHubId,
            DestHubId = DestHubId,
            CurrentHubId = currentHubId,
            ServiceTypeId = ServiceTypeId,
            Status = status,
            DeliveryAttempts = deliveryAttempts,
            CodAmount = new Money(150000, "VND"),
            SenderAddress = new Address("Street A", "Ha Noi", "Dong Da", "Ward 1"),
            ReceiverAddress = new Address("Street B", "Ho Chi Minh", "Quan 1", "Ward 2"),
            Weight = 2.5m
        };

        Context.Parcels.Add(parcel);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
        return parcel;
    }

    public async Task<Bag> SeedBagAsync(BagStatus status, string bagCode, int parcelCount = 0)
    {
        var bag = new Bag
        {
            Id = Guid.NewGuid(),
            BagCode = bagCode,
            FromHubId = OriginHubId,
            ToHubId = DestHubId,
            Status = status,
            ParcelCount = parcelCount,
            SealedAt = status == BagStatus.Sealed ? DateTime.UtcNow : null
        };

        Context.Bags.Add(bag);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
        return bag;
    }

    public async Task SeedBagItemAsync(Guid bagId, Guid parcelId)
    {
        Context.BagItems.Add(new BagItem
        {
            Id = Guid.NewGuid(),
            BagId = bagId,
            ParcelId = parcelId,
            AddedAt = DateTime.UtcNow,
            AddedBy = CurrentUser.UserId ?? Guid.Empty
        });

        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public async Task<Parcel?> ReloadParcelAsync(Guid parcelId)
    {
        Context.ChangeTracker.Clear();
        return await Context.Parcels.FirstOrDefaultAsync(x => x.Id == parcelId);
    }

    public async Task<Parcel> ReloadParcelAsync(string trackingCode)
    {
        Context.ChangeTracker.Clear();
        return await Context.Parcels.FirstAsync(x => x.TrackingCode == trackingCode);
    }

    public async Task<Bag?> ReloadBagAsync(Guid bagId)
    {
        Context.ChangeTracker.Clear();
        return await Context.Bags.FirstOrDefaultAsync(x => x.Id == bagId);
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }

    private void SeedCoreData()
    {
        Context.Merchants.Add(new Merchant
        {
            Id = MerchantId,
            Name = "Probe Merchant",
            MerchantCode = "M-PROBE"
        });

        Context.Hubs.Add(new Hub
        {
            Id = OriginHubId,
            Name = "Origin Hub",
            HubCode = "HUB-A"
        });

        Context.Hubs.Add(new Hub
        {
            Id = DestHubId,
            Name = "Destination Hub",
            HubCode = "HUB-D"
        });

        Context.ServiceTypes.Add(new ServiceType
        {
            Id = ServiceTypeId,
            Name = "Standard",
            Code = "SERVICE_STANDARD"
        });

        Context.Users.Add(new User
        {
            Id = ShipperId,
            EmployeeCode = "SHP-001",
            FullName = "Probe Shipper",
            Phone = "0901000001",
            PasswordHash = "123456",
            Role = UserRole.Shipper,
            HubId = DestHubId,
            IsActive = true
        });

        Context.SaveChanges();
        Context.ChangeTracker.Clear();
    }
}

internal sealed class ProbeCurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; set; }
    public Guid? MerchantId { get; set; }
    public string? Role { get; set; }
}

internal sealed class CaptureLogger<T> : ILogger<T>
{
    public List<string> Messages { get; } = new();

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return NullScope.Instance;
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        Messages.Add($"{logLevel}: {formatter(state, exception)}");
        if (exception != null)
        {
            Messages.Add($"{exception.GetType().Name}: {exception.Message}");
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
