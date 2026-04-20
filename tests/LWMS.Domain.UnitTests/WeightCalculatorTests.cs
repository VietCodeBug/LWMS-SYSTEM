using FluentAssertions;
using LWMS.Domain.Services;

namespace LWMS.Domain.UnitTests;

public class WeightCalculatorTests
{
    [Theory]
    [InlineData(10, 10, 10, 0.2)]      // (10 * 10 * 10) / 5000 = 1000 / 5000 = 0.2
    [InlineData(50, 40, 30, 12)]       // (50 * 40 * 30) / 5000 = 60000 / 5000 = 12
    [InlineData(100, 50, 50, 50)]      // (100 * 50 * 50) / 5000 = 250000 / 5000 = 50
    public void CalculateDimensionalWeight_ShouldReturnCorrectValue(decimal length, decimal width, decimal height, decimal expectedResult)
    {
        // Act
        var result = WeightCalculator.CalculateDimensionalWeight(length, width, height);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(5, 10, 10)]            // Actual is smaller
    [InlineData(15, 10, 15)]           // Actual is larger
    [InlineData(10, 10, 10)]           // Both are equal
    public void GetChargeableWeight_ShouldReturnMaxOfActualAndDimensional(decimal actual, decimal dimensional, decimal expectedResult)
    {
        // Act
        var result = WeightCalculator.GetChargeableWeight(actual, dimensional);

        // Assert
        result.Should().Be(expectedResult);
    }
}
