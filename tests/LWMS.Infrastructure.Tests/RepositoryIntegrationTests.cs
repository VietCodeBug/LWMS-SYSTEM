using FluentAssertions;
using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Domain.Enums;
using LWMS.Domain.ValueObjects;
using LWMS.Infrastructure.Data;
using LWMS.Infrastructure.Data.Interceptors;
using LWMS.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace LWMS.Infrastructure.Tests;

public class RepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _uow;
    private readonly SqliteConnection _connection;
    private readonly ITestOutputHelper _output;

    private Guid _merchantId;
    private Guid _hubId;
    private Guid _serviceTypeId;

    public RepositoryIntegrationTests(ITestOutputHelper output)
    {
        _output = output;

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var currentUserServiceMock = new Mock<ICurrentUserService>();
        currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        currentUserServiceMock.Setup(x => x.Role).Returns("Admin");

        var interceptor = new AuditInterceptor(currentUserServiceMock.Object);

        _context = new AppDbContext(options, currentUserServiceMock.Object, interceptor);
        _context.Database.EnsureCreated();

        _uow = new UnitOfWork(_context);
        
        SetupSeedData();
    }

    private void SetupSeedData()
    {
        // Tạo dữ liệu mồi để vượt qua FK constraint
        var merchant = new Merchant { Id = Guid.NewGuid(), Name = "Test Merchant", MerchantCode = "M-001" };
        var hub = new Hub { Id = Guid.NewGuid(), Name = "Test Hub", HubCode = "H-001" };
        var serviceType = new ServiceType { Id = Guid.NewGuid(), Name = "Standard", Code = "STD" };

        _context.Merchants.Add(merchant);
        _context.Hubs.Add(hub);
        _context.ServiceTypes.Add(serviceType);
        _context.SaveChanges();

        _merchantId = merchant.Id;
        _hubId = hub.Id;
        _serviceTypeId = serviceType.Id;
        
        _context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task CreateParcel_Should_GenerateAuditLog_Successfully()
    {
        _output.WriteLine("🚀 Bắt đầu test: CreateParcel_Should_GenerateAuditLog");

        // Arrange
        var parcel = new Parcel
        {
            Id = Guid.NewGuid(),
            TrackingCode = "TEST-TRK-" + Guid.NewGuid().ToString().Substring(0, 8),
            SenderName = "Antigravity",
            ReceiverName = "User",
            MerchantId = _merchantId,
            OriginHubId = _hubId,
            DestHubId = _hubId,
            ServiceTypeId = _serviceTypeId,
            Status = ParcelStatus.Created,
            CodAmount = new Money(100000, "VND"),
            SenderAddress = new Address("Street A", "Prov A", "Dist A", "Ward A"),
            ReceiverAddress = new Address("Street B", "Prov B", "Dist B", "Ward B")
        };

        // Act
        _output.WriteLine("📦 Đang thêm đơn hàng vào UnitOfWork...");
        await _uow.Parcels.AddAsync(parcel);
        await _uow.SaveChangesAsync();

        // Assert
        _output.WriteLine("📝 Kiểm tra dữ liệu trong DB...");
        var savedParcel = await _uow.Parcels.GetByIdAsync(parcel.Id);
        savedParcel.Should().NotBeNull();

        _output.WriteLine("🔍 Kiểm tra bảng AuditLogs...");
        var auditLogs = await _context.AuditLogs.Where(a => a.EntityId == parcel.Id).ToListAsync();
        auditLogs.Should().NotBeEmpty();
        auditLogs.Should().Contain(x => x.Action == "ADDED" && x.EntityType == "Parcel");

        _output.WriteLine($"✅ Thành công! Audit Action: {auditLogs.First().Action}");
    }

    [Fact]
    public async Task UpdateParcel_Should_LogChanges_Precisely()
    {
        _output.WriteLine("🚀 Bắt đầu test: UpdateParcel_Should_LogChanges");

        // Arrange
        var parcelId = Guid.NewGuid();
        var parcel = new Parcel 
        { 
            Id = parcelId, 
            TrackingCode = "TRK-" + Guid.NewGuid().ToString().Substring(0,8), 
            Status = ParcelStatus.Created, 
            MerchantId = _merchantId,
            OriginHubId = _hubId,
            DestHubId = _hubId,
            ServiceTypeId = _serviceTypeId,
            CodAmount = new Money(50000, "VND"),
            SenderAddress = new Address("A", "P", "D", "W"),
            ReceiverAddress = new Address("A", "P", "D", "W")
        };
        _context.Parcels.Add(parcel);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var parcelToUpdate = await _uow.Parcels.GetByIdAsync(parcelId);
        parcelToUpdate!.Status = ParcelStatus.InTransit;
        _uow.Parcels.Update(parcelToUpdate);
        await _uow.SaveChangesAsync();

        // Assert
        var logs = await _context.AuditLogs
            .Where(x => x.Action == "MODIFIED" && x.EntityId == parcelId)
            .ToListAsync();

        logs.Should().NotBeEmpty();
        logs.First().Changes.Should().Contain("InTransit");
        _output.WriteLine($"✅ Logs update: {logs.First().Changes}");
    }

    [Fact]
    public async Task Bagging_Process_Should_Succeed()
    {
        _output.WriteLine("🚀 Bắt đầu test: Quy trình đóng bao (Bagging Process)");

        // 1. Tạo Bag mới
        var bag = new Bag
        {
            Id = Guid.NewGuid(),
            BagCode = "BAG-TEST-001",
            FromHubId = _hubId,
            ToHubId = _hubId,
            Status = BagStatus.Open
        };
        await _uow.Bags.AddAsync(bag);

        // 2. Tạo 2 kiện hàng để cho vào bao
        var p1 = CreateTestParcel("TRK-BAG-001");
        var p2 = CreateTestParcel("TRK-BAG-002");
        await _uow.Parcels.AddAsync(p1);
        await _uow.Parcels.AddAsync(p2);

        // 3. Đóng kiện hàng vào bao (BagItem)
        await _uow.BagItems.AddAsync(new BagItem { Id = Guid.NewGuid(), BagId = bag.Id, ParcelId = p1.Id });
        await _uow.BagItems.AddAsync(new BagItem { Id = Guid.NewGuid(), BagId = bag.Id, ParcelId = p2.Id });

        // 4. Kẹp chì (Seal Bag)
        bag.Status = BagStatus.Sealed;
        bag.SealNumber = "SEAL-123456";
        bag.SealedAt = DateTime.UtcNow;

        // 5. Lưu toàn bộ (EF sẽ tự động Insert Bag, Parcels, BagItems và cập nhật trạng thái Bag trong 1 transaction)
        await _uow.SaveChangesAsync();

        // ASSERT
        _output.WriteLine("🔍 Đang kiểm tra kết quả đóng bao...");
        var savedBag = await _uow.Bags.GetByIdAsync(bag.Id);
        savedBag!.Status.Should().Be(BagStatus.Sealed);
        savedBag.SealNumber.Should().Be("SEAL-123456");

        var items = await _context.BagItems.Where(x => x.BagId == bag.Id).ToListAsync();
        items.Should().HaveCount(2);

        var audit = await _context.AuditLogs
            .Where(a => a.EntityType == "Bag" && a.Action == "ADDED")
            .FirstOrDefaultAsync();
        audit.Should().NotBeNull();
        _output.WriteLine($"✅ Thành công! Audit Log ghi nhận Bag mới khởi tạo đã có Status: {audit!.Changes}");
    }

    private Parcel CreateTestParcel(string trk) => new Parcel
    {
        Id = Guid.NewGuid(),
        TrackingCode = trk,
        SenderName = "S", ReceiverName = "R",
        MerchantId = _merchantId, OriginHubId = _hubId, DestHubId = _hubId, ServiceTypeId = _serviceTypeId,
        CodAmount = new Money(0, "VND"),
        SenderAddress = new Address("A", "P", "D", "W"),
        ReceiverAddress = new Address("A", "P", "D", "W")
    };

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
