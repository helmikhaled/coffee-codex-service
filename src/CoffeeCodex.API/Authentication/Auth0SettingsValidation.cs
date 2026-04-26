using Microsoft.Extensions.Options;

namespace CoffeeCodex.API.Authentication;

internal sealed class Auth0SettingsValidation : IValidateOptions<Auth0Settings>
{
    public ValidateOptionsResult Validate(string? name, Auth0Settings options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Domain))
        {
            failures.Add("Missing configuration value 'Auth0:Domain'.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            failures.Add("Missing configuration value 'Auth0:Audience'.");
        }

        if (options.Domain.Length > 0)
        {
            if (!Uri.TryCreate(options.Authority, UriKind.Absolute, out var authority))
            {
                failures.Add("Configuration value 'Auth0:Domain' must be a valid absolute URI or host name.");
            }
            else
            {
                if (!string.Equals(authority.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add("Configuration value 'Auth0:Domain' must use HTTPS.");
                }

                if (string.IsNullOrWhiteSpace(authority.Host))
                {
                    failures.Add("Configuration value 'Auth0:Domain' must include a host.");
                }

                if (!string.Equals(authority.AbsolutePath, "/", StringComparison.Ordinal))
                {
                    failures.Add("Configuration value 'Auth0:Domain' must not include a path.");
                }

                if (!string.IsNullOrEmpty(authority.Query) || !string.IsNullOrEmpty(authority.Fragment))
                {
                    failures.Add("Configuration value 'Auth0:Domain' must not include a query string or fragment.");
                }
            }
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
