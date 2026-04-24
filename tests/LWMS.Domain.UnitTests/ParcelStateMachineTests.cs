using FluentAssertions;
using LWMS.Domain.Enums;
using LWMS.Domain.Services;

namespace LWMS.Domain.UnitTests;

public class ParcelStateMachineTests
{
    [Theory]
    // Happy Path: Merchant flows
    [InlineData(ParcelStatus.Created, ParcelStatus.Picking)]
    [InlineData(ParcelStatus.Picking, ParcelStatus.Picked)]
    [InlineData(ParcelStatus.Picked, ParcelStatus.ArrivedHub)]
    [InlineData(ParcelStatus.Created, ParcelStatus.LabelPrinted)]
    [InlineData(ParcelStatus.Created, ParcelStatus.ArrivedHub)]
    [InlineData(ParcelStatus.LabelPrinted, ParcelStatus.ArrivedHub)]

    // Hub Flow
    [InlineData(ParcelStatus.ArrivedHub, ParcelStatus.Sorted)]
    [InlineData(ParcelStatus.ArrivedHub, ParcelStatus.InBag)]
    [InlineData(ParcelStatus.ArrivedHub, ParcelStatus.OutForDelivery)]
    
    // Baggage & Transit
    [InlineData(ParcelStatus.Sorted, ParcelStatus.InBag)]
    [InlineData(ParcelStatus.Sorted, ParcelStatus.InTransit)]
    [InlineData(ParcelStatus.InBag, ParcelStatus.InTransit)]
    [InlineData(ParcelStatus.InTransit, ParcelStatus.ArrivedHub)]

    // Delivery Flow
    [InlineData(ParcelStatus.OutForDelivery, ParcelStatus.Delivered)]
    [InlineData(ParcelStatus.OutForDelivery, ParcelStatus.FailedDelivery)]
    [InlineData(ParcelStatus.FailedDelivery, ParcelStatus.OutForDelivery)]
    [InlineData(ParcelStatus.FailedDelivery, ParcelStatus.Returning)]
    [InlineData(ParcelStatus.Returning, ParcelStatus.Returned)]
    [InlineData(ParcelStatus.ArrivedHub, ParcelStatus.Returning)] // Admin can force returning
    [InlineData(ParcelStatus.Created, ParcelStatus.Cancelled)]
    public void CanTransition_WithValidTransitions_ShouldReturnTrue(ParcelStatus from, ParcelStatus to)
    {
        // Act
        var result = ParcelStateMachine.CanTransition(from, to);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(ParcelStatus.Created, ParcelStatus.Delivered)]
    [InlineData(ParcelStatus.Returned, ParcelStatus.Created)]
    [InlineData(ParcelStatus.Delivered, ParcelStatus.Returned)]
    [InlineData(ParcelStatus.InBag, ParcelStatus.Delivered)]
    public void CanTransition_WithInvalidTransitions_ShouldReturnFalse(ParcelStatus from, ParcelStatus to)
    {
        // Act
        var result = ParcelStateMachine.CanTransition(from, to);

        // Assert
        result.Should().BeFalse();
    }
}
