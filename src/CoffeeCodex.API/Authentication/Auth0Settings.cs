namespace CoffeeCodex.API.Authentication;

public sealed class Auth0Settings
{
    public const string SectionName = "Auth0";

    public string Domain { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string ClientId { get; init; } = string.Empty;

    public string ClientSecret { get; init; } = string.Empty;

    public string Authority => NormalizeDomain(Domain);

    public string Issuer => Authority;

    public static string NormalizeDomain(string domain)
    {
        var normalizedDomain = domain.Trim();

        if (normalizedDomain.Length == 0)
        {
            return string.Empty;
        }

        if (!normalizedDomain.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            && !normalizedDomain.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
        {
            normalizedDomain = $"https://{normalizedDomain}";
        }

        if (!normalizedDomain.EndsWith('/'))
        {
            normalizedDomain = $"{normalizedDomain}/";
        }

        return normalizedDomain;
    }
}
