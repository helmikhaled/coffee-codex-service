using System.Net;
using System.Net.Http.Headers;

namespace CoffeeCodex.RecipeListing.Tests.Api;

public sealed class AuthenticationEndpointTest : IClassFixture<AuthenticationApiFactory>
{
    private readonly HttpClient _httpClient;

    public AuthenticationEndpointTest(AuthenticationApiFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetAuthCheck_WhenTokenIsMissing_ReturnsUnauthorized()
    {
        using var response = await _httpClient.GetAsync("/admin/auth-check");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthCheck_WhenTokenIsMalformed_ReturnsUnauthorized()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/admin/auth-check");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "not-a-jwt");
        using var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthCheck_WhenIssuerIsWrong_ReturnsUnauthorized()
    {
        using var request = CreateRequest(AuthTestTokenFactory.CreateToken(issuer: "https://unexpected-issuer.auth0.local/"));
        using var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthCheck_WhenAudienceIsWrong_ReturnsUnauthorized()
    {
        using var request = CreateRequest(AuthTestTokenFactory.CreateToken(audience: "https://unexpected-audience"));
        using var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthCheck_WhenTokenIsExpired_ReturnsUnauthorized()
    {
        using var request = CreateRequest(AuthTestTokenFactory.CreateToken(expiresAt: DateTimeOffset.UtcNow.AddMinutes(-5)));
        using var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthCheck_WhenTokenIsValid_ReturnsNoContent()
    {
        using var request = CreateRequest(AuthTestTokenFactory.CreateToken());
        using var response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static HttpRequestMessage CreateRequest(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/admin/auth-check");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }
}
