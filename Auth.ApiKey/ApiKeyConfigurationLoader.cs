using Microsoft.Extensions.Configuration;

namespace Auth.ApiKey;

public class ApiKeyConfigurationLoader
{
    private const string APIKEYS_CONFIGURATION_NAME = "ApiKeys";

    public readonly IReadOnlyList<string> ApiKeys;

    public ApiKeyConfigurationLoader(
        IConfiguration configuration)
    {
        var apiKeys = configuration.GetSection(APIKEYS_CONFIGURATION_NAME);
        if (apiKeys == null)
            throw new Exception($"API Key section {APIKEYS_CONFIGURATION_NAME} must be specified");

        ApiKeys = apiKeys.GetChildren().Select(key => key.Value).ToList<string>();
    }
}
