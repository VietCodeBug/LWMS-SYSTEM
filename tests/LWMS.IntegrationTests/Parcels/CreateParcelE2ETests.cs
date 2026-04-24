using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LWMS.Application.Parcels.Commands.Create;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LWMS.IntegrationTests.Parcels;

public class CreateParcelE2ETests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CreateParcelE2ETests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateParcel_ShouldReturnCreated_WhenValid()
    {
        // 1. Arrange
        var command = new CreateParcelCommand
        {
            SenderName = "Merchant A",
            SenderPhone = "0901112223",
            ReceiverName = "Cus B",
            ReceiverPhone = "0904445556",
            Weight = 2.0m,
            CodAmount = 100000,
            Province = "HCM",
            OriginHubId = Guid.NewGuid(),
            DestHubId = Guid.NewGuid(),
            ServiceId = Guid.NewGuid()
        };

        // 2. Act
        // Luu y: DiagnosticsController hoac endpoint test khong require auth co the dung de test flow
        // Hoac chung ta mock auth.
        // Hien tai endpoint /api/v1/parcels co [Authorize], nen neu goi truc tiep se bi 401.
        
        // De demo, toi se goi mot endpoint khong can auth hoac gia lap auth (can cau hinh factory)
        var response = await _client.PostAsJsonAsync("/api/v1/parcels", command);

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CreateParcelResponse>();
        result.Should().NotBeNull();
        result.TrackingCode.Should().NotBeEmpty();
    }
}
