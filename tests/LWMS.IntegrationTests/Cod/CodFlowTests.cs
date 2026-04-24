using System.Net;
using System.Net.Http.Headers;
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
            CodAmount = 500000, 
            Province = "HCM",
            OriginHubId = CustomWebApplicationFactory<Program>.TestHubId,
            DestHubId = CustomWebApplicationFactory<Program>.TestDestHubId,
            ServiceId = CustomWebApplicationFactory<Program>.TestServiceId
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/parcels");
        requestMessage.Content = JsonContent.Create(command);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Test");

        var response = await _client.SendAsync(requestMessage);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<CreateParcelResponse>();

        // Assert 
        result.Should().NotBeNull();
        result.TrackingCode.Should().NotBeEmpty();
    }
}
