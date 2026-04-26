using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace CoffeeCodex.RecipeListing.Tests.Api;

internal static class AuthTestTokenFactory
{
    private static readonly RSA SigningKeyRsa = RSA.Create(2048);
    private static readonly RsaSecurityKey SigningKey = new(SigningKeyRsa);
    private static readonly SigningCredentials SigningCredentials = new(SigningKey, SecurityAlgorithms.RsaSha256);

    public const string Issuer = "https://coffee-codex-tests.auth0.local/";

    public const string Audience = "https://coffee-codex-tests-api";

    public static SecurityKey ValidationKey => SigningKey;

    public static string CreateToken(
        string? issuer = null,
        string? audience = null,
        DateTimeOffset? expiresAt = null)
    {
        var now = DateTimeOffset.UtcNow;
        var expires = expiresAt ?? now.AddMinutes(5);
        var notBefore = expires < now
            ? expires.AddMinutes(-5)
            : now.AddMinutes(-1);
        var token = new JwtSecurityToken(
            issuer: issuer ?? Issuer,
            audience: audience ?? Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString("N")),
            ],
            notBefore: notBefore.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: SigningCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
