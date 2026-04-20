using FluentAssertions;
using LWMS.Domain.Services;

namespace LWMS.Domain.UnitTests;

public class TrackingCodeGeneratorTests
{
    [Theory]
    [InlineData("HN01", 1234, "HN01")]
    [InlineData("HCM02", 5, "HCM02")]
    public void GenerateTrackingCode_ShouldReturnCorrectFormat(string hubCode, int sequence, string expectedHubPrefix)
    {
        // Arrange
        var dateStr = DateTime.UtcNow.ToString("yyyyMMdd");
        var expectedSeqStr = sequence.ToString().PadLeft(6, '0');
        var expectedCode = $"{expectedHubPrefix}-{dateStr}-{expectedSeqStr}";

        // Act
        var result = TrackingCodeGenerator.GenerateTrackingCode(hubCode, sequence);

        // Assert
        result.Should().Be(expectedCode);
        result.Should().StartWith($"{expectedHubPrefix}-");
        result.Should().EndWith($"-{expectedSeqStr}");
    }
}
