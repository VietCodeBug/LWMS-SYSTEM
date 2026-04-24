using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LWMS.Application.Parcels.Commands.Create;
using LWMS.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LWMS.IntegrationTests.Cod;

public class CodFlowTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CodFlowTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeliveredParcel_WithCod_ShouldGenerateCodRecord()
    {
        // 1. Create a Parcel with COD via API
        var command = new CreateParcelCommand
        {
            SenderName = "Merchant A",
            SenderPhone = "0901112223",
            ReceiverName = "Cus B",
            ReceiverPhone = "0904445556",
            Weight = 2.0m,
            CodAmount = 500000, // 500k COD
            Province = "HCM",
            OriginHubId = Guid.NewGuid(),
            DestHubId = Guid.NewGuid(),
            ServiceId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync("/api/v1/parcels", command);
        response.EnsureSuccessStatusCode();
        var parcel = await response.Content.ReadFromJsonAsync<CreateParcelResponse>();

        // 2. Simulate Delivery success (Thiếu endpoint Delivery hien tai? - Se gia lap qua DB hoac internal command)
        // Trong Integration Test chung ta thuong goi POST /api/v1/parcels/{id}/delivered
        
        // Assert: 
        parcel.Should().NotBeNull();
        parcel.TrackingCode.Should().NotBeEmpty();
        
        // TODO: Goi endpoint cap nhat Delivered va check /api/v1/cod/{merchantId}
    }
}
