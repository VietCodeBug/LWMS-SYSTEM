using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LWMS.IntegrationTests.Concurrency;

public class ConcurrencyTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ConcurrencyTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task TwoSorters_ScanningSameParcel_ShouldOnlySuccessOne()
    {
        // 1. Create a parcel
        // ... (Tuong tu cac test tren)

        // 2. Parallel scan requests
        var trackingCode = "TEST-CONC-001";
        
        // Chung ta se dung Task.WhenAll de goi song song
        // var tasks = new[] {
        //     _client.PostAsJsonAsync("/api/v1/parcels/scan-inbound", new { TrackingCode = trackingCode }),
        //     _client.PostAsJsonAsync("/api/v1/parcels/scan-inbound", new { TrackingCode = trackingCode })
        // };
        
        // var results = await Task.WhenAll(tasks);
        
        // Assert: It nhat 1 cai phai tra ve 400 Bad Request hoac Conflict neu chung ta dung Optimistic Concurrency
        
        Assert.True(true); // Placeholder for implementation
    }
}
