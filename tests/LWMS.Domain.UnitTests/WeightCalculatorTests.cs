using FluentAssertions;
using LWMS.Domain.Services;

namespace LWMS.Domain.UnitTests;

public class WeightCalculatorTests
{
    [Theory]
    [InlineData(10, 10, 10, 0.2)] // (10*10*10)/5000 = 1000/5000 = 0.2
    [InlineData(50, 50, 40, 20)]   // (50*50*40)/5000 = 100000/5000 = 20
    [InlineData(100, 100, 100, 200)] // (100^3)/5000 = 1000000/5000 = 200
    public void CalculateDimensionalWeight_ShouldReturnCorrectValue(decimal l, decimal w, decimal h, decimal expected)
    {
        // Act
        var result = WeightCalculator.CalculateDimensionalWeight(l, w, h);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(10, 5, 10)] // Actual > Dim
    [InlineData(5, 10, 10)] // Dim > Actual
    [InlineData(8, 8, 8)]   // Equal
    public void GetChargeableWeight_ShouldReturnMax(decimal actual, decimal dim, decimal expected)
    {
        // Act
        var result = WeightCalculator.GetChargeableWeight(actual, dim);

        // Assert
        result.Should().Be(expected);
    }
}
