using Moq;
using LWMS.Application.Parcels.Commands.Create;
using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using FluentAssertions;
using LWMS.Domain.Enums;

namespace LWMS.Application.UnitTests.Parcels;

public class CreateParcelCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CreateParcelCommandHandler _handler;

    public CreateParcelCommandHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        
        // Mock Hubs
        var hubRepoMock = new Mock<IHubRepository>();
        hubRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Hub { Name = "Test Hub" });
        _uowMock.Setup(x => x.Hubs).Returns(hubRepoMock.Object);

        // Mock Parcels
        _uowMock.Setup(x => x.Parcels).Returns(new Mock<IParcelRepository>().Object);

        // Mock TrackingLogs
        _uowMock.Setup(x => x.TrackingLogs).Returns(new Mock<IRepository<TrackingLog>>().Object);

        _handler = new CreateParcelCommandHandler(_uowMock.Object, _currentUserServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateParcelAndTrackingLog()
    {
        // Arrange
        var command = new CreateParcelCommand
        {
            ReceiverPhone = "0901234567",
            Weight = 1.5m,
            CodAmount = 500000,
            OriginHubId = Guid.NewGuid(),
            DestHubId = Guid.NewGuid(),
            ServiceId = Guid.NewGuid(),
            Province = "Ha Noi"
        };
        
        _currentUserServiceMock.Setup(x => x.MerchantId).Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TrackingCode.Should().NotBeEmpty();
        _uowMock.Verify(x => x.Parcels.AddAsync(It.IsAny<Parcel>()), Times.Once);
        _uowMock.Verify(x => x.TrackingLogs.AddAsync(It.IsAny<TrackingLog>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
}
