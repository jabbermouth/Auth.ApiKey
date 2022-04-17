using Microsoft.AspNetCore.Authorization;

namespace Auth.ApiKey;

public class ApiKeyRequirement : IAuthorizationRequirement
{
    public IReadOnlyList<string> ApiKeys { get; set; }

    public ApiKeyRequirement(IEnumerable<string> apiKeys)
    {
        ApiKeys = apiKeys?.ToList() ?? new List<string>();
    }

}