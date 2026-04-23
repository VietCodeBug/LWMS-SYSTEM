using LWMS.Application.Common.Behaviors;
using LWMS.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LWMS.Tests.Behaviors;

public class LoggingBehaviorTests
{
    public class TestRequest : IRequest<string>
    {
    }

    [Fact]
    public async Task Handle_Should_Log_Start_And_Success()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, string>>>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();
        var expectedUserId = Guid.NewGuid();
        currentUserServiceMock.Setup(x => x.UserId).Returns(expectedUserId);
        
        var behavior = new LoggingBehavior<TestRequest, string>(loggerMock.Object, currentUserServiceMock.Object);
        var request = new TestRequest();
        var cancellationToken = new CancellationToken();

        RequestHandlerDelegate<string> next = () => Task.FromResult("SuccessResult");

        // Act
        var response = await behavior.Handle(request, next, cancellationToken);

        // Assert
        Assert.Equal("SuccessResult", response);
        
        // We verify that logger is called at least once
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("START")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SUCCESS")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}
