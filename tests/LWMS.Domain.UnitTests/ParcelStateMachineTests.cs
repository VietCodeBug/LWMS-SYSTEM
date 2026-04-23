using FluentAssertions;
using LWMS.Domain.Enums;
using LWMS.Domain.Services;

namespace LWMS.Domain.UnitTests;

public class ParcelStateMachineTests
{
    [Theory]
    [InlineData(ParcelStatus.Created, ParcelStatus.Picking)]
    [InlineData(ParcelStatus.Picking, ParcelStatus.Picked)]
    [InlineData(ParcelStatus.Picked, ParcelStatus.InTransit)]
    [InlineData(ParcelStatus.InTransit, ParcelStatus.ArrivedHub)]
    [InlineData(ParcelStatus.ArrivedHub, ParcelStatus.OutForDelivery)]
    [InlineData(ParcelStatus.OutForDelivery, ParcelStatus.Delivered)]
    [InlineData(ParcelStatus.OutForDelivery, ParcelStatus.FailedDelivery)]
    [InlineData(ParcelStatus.FailedDelivery, ParcelStatus.Returning)]
    [InlineData(ParcelStatus.Returning, ParcelStatus.Returned)]
    public void CanTransition_WithValidTransitions_ShouldReturnTrue(ParcelStatus from, ParcelStatus to)
    {
        // Act
        var result = ParcelStateMachine.CanTransition(from, to);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(ParcelStatus.Created, ParcelStatus.Delivered)]
    [InlineData(ParcelStatus.Returning, ParcelStatus.Delivered)]
    [InlineData(ParcelStatus.Delivered, ParcelStatus.Returned)]
    [InlineData(ParcelStatus.Created, ParcelStatus.Returned)]
    [InlineData(ParcelStatus.Picked, ParcelStatus.Delivered)]
    [InlineData(ParcelStatus.OutForDelivery, ParcelStatus.Created)]
    [InlineData(ParcelStatus.ArrivedHub, ParcelStatus.Returned)]
    public void CanTransition_WithInvalidTransitions_ShouldReturnFalse(ParcelStatus from, ParcelStatus to)
    {
        // Act
        var result = ParcelStateMachine.CanTransition(from, to);

        // Assert
        result.Should().BeFalse();
    }
}
