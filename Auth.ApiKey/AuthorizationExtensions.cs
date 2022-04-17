using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Auth.ApiKey;

public static class AuthorizationExtensions
{
    private const string APIKEYS_CONFIGURATION_NAME = "ApiKeys";

    public static void AddApiPolicy(this AuthorizationOptions authOptions, ConfigurationManager configuration, string apiKeySection = APIKEYS_CONFIGURATION_NAME)
    {
        var apiKeysInConfig = configuration.GetSection(apiKeySection);
        if (apiKeysInConfig == null)
            throw new Exception($"API Key section {apiKeySection} must be specified");

        var apiKeys = apiKeysInConfig.GetChildren().Select(key => key.Value).ToList<string>();

        authOptions.AddPolicy(ApiKeyRequirementHandler.ApiKeyPolicySettingName,
                policyBuilder => policyBuilder.Requirements.Add(new ApiKeyRequirement(apiKeys)));
    }
}
