using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace LWMS.IntegrationTests;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Chỉ xác thực nếu có header Authorization: Test
        if (!Request.Headers.ContainsKey("Authorization") || Request.Headers["Authorization"] != "Test")
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing or invalid authorization header for Test scheme."));
        }

        var testUserId = CustomWebApplicationFactory<Program>.TestUserId.ToString();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("MerchantId", CustomWebApplicationFactory<Program>.TestMerchantId.ToString()),
            
            // Add multiple variations of NameIdentifier to ensure ICurrentUserService finds it
            new Claim(ClaimTypes.NameIdentifier, testUserId),
            new Claim("sub", testUserId),
            new Claim("userId", testUserId),
            new Claim("uid", testUserId)
        };
        
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
