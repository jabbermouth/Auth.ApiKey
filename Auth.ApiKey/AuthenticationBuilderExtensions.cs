using Microsoft.AspNetCore.Authentication;

namespace Auth.ApiKey;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddApiKeySupport(this AuthenticationBuilder authenticationBuilder)
    {
        return authenticationBuilder
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>
            (ApiKeyAuthenticationOptions.DefaultScheme, null);
    }

    public static AuthenticationBuilder AddApiKeySupport(this AuthenticationBuilder authenticationBuilder, Action<ApiKeyAuthenticationOptions> options)
    {
        return authenticationBuilder
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>
            (ApiKeyAuthenticationOptions.DefaultScheme, options);
    }
}
