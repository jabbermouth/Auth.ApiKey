using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;

//[assembly: InternalsVisibleTo("PomsSubset.Api.Tests")]
namespace Auth.ApiKey;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private const string APIKEYS_CONFIGURATION_NAME = "ApiKeys";

    private readonly IReadOnlyList<string> _apiKeys;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration) : base(options, logger, encoder, clock)
    {
        var apiKeys = configuration.GetSection(APIKEYS_CONFIGURATION_NAME);
        if (apiKeys == null)
            throw new Exception($"API Key section {APIKEYS_CONFIGURATION_NAME} must be specified");

        _apiKeys = apiKeys.GetChildren().Select(key => key.Value).ToList<string>();
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return ProcessAuthenticateResult();
    }

    internal AuthenticateResult ProcessAuthenticateResult()
    {
        if (!Request.Headers.TryGetValue(
            ApiKeyRequirementHandler.ApikeyHeaderName,
            out StringValues apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        string providedApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
        {
            return AuthenticateResult.NoResult();
        }

        if (!_apiKeys.Any(key => key.Equals(providedApiKey)))
            return AuthenticateResult.Fail("Invalid API Key provided.");

        string apiKey = providedApiKey;

        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name,apiKey)
        };

        ClaimsIdentity identity = new ClaimsIdentity(claims, Options.AuthenticationType);
        List<ClaimsIdentity> identities = new List<ClaimsIdentity> { identity };
        ClaimsPrincipal principal = new ClaimsPrincipal(identities);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, Options.Scheme);

        return AuthenticateResult.Success(ticket);
    }
}
