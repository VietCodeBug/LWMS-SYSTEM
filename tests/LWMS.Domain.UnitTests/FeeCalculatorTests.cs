using FluentAssertions;
using LWMS.Domain.Services;

namespace LWMS.Domain.UnitTests;

public class FeeCalculatorTests
{
    [Theory]
    [InlineData(500, 15000)]       // <= 1000g => 15000
    [InlineData(1000, 15000)]      // <= 1000g => 15000
    [InlineData(1001, 20000)]      // Extra 1kg => +5000 = 20000
    [InlineData(1500, 20000)]      // Extra 1kg => +5000 = 20000
    [InlineData(2000, 20000)]      // Extra 1kg => +5000 = 20000
    [InlineData(2001, 25000)]      // Extra 2kg => +10000 = 25000
    [InlineData(5500, 40000)]      // Extra 5kg => +25000 = 40000
    public void CalculateShippingFee_ShouldReturnCorrectValue(decimal weight, decimal expectedFee)
    {
        // Act
        var result = FeeCalculator.CalculateShippingFee(weight);

        // Assert
        result.Should().Be(expectedFee);
    }

    [Theory]
    [InlineData(100000, 1000)]     // 100000 * 0.01 = 1000
    [InlineData(500000, 5000)]     // 500000 * 0.01 = 5000
    [InlineData(0, 0)]             // 0 * 0.01 = 0
    public void CalculateCodFee_ShouldReturnCorrectValue(decimal codAmount, decimal expectedFee)
    {
        // Act
        var result = FeeCalculator.CalculateCodFee(codAmount);

        // Assert
        result.Should().Be(expectedFee);
    }

    [Theory]
    [InlineData(500, 100000, 16000)]     // Shipping 15000 + COD 1000
    [InlineData(1500, 200000, 22000)]    // Shipping 20000 + COD 2000
    [InlineData(1000, 0, 15000)]         // Shipping 15000 + COD 0
    public void CalculateTotalFee_ShouldReturnShippingFeePlusCodFee(decimal weight, decimal codAmount, decimal expectedTotalFee)
    {
        // Act
        var result = FeeCalculator.CalculateTotalFee(weight, codAmount);

        // Assert
        result.Should().Be(expectedTotalFee);
    }
}
