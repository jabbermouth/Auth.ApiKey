using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Auth.ApiKey;

public class ApiKeyRequirementHandler : AuthorizationHandler<ApiKeyRequirement>
{
    public const string ApiKeyPolicySettingName = "ApiKeyPolicy";
    public const string ApikeyHeaderName = "X-API-KEY";

    private readonly IHttpContextAccessor _httpContextAccessor;

    private ILogger<ApiKeyRequirementHandler> _logger;

    public ApiKeyRequirementHandler(IHttpContextAccessor httpContextAccessor, ILogger<ApiKeyRequirementHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
    {
        SucceedRequirementIfApiKeyPresentAndValid(context, requirement);
        return Task.CompletedTask;
    }

    private void SucceedRequirementIfApiKeyPresentAndValid(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return;
        }

        var query = httpContext.Request.Headers;
        string apiKey = query.Where(x => x.Key.ToLowerInvariant() == ApikeyHeaderName.ToLowerInvariant().ToString())
            .FirstOrDefault().Value;

        if (apiKey != null && requirement.ApiKeys.Any(key => key.Equals(apiKey)))
        {
            context.Succeed(requirement);
        }
        else
        {
            //explicitly fail the authorisation
            string keyToDisplay;
            if (apiKey is null)
            {
                keyToDisplay = "NULL";
            }
            else if (apiKey.Length > 4)
            {
                keyToDisplay = apiKey.Substring(0, 4) + "...";
            }
            else
            {
                keyToDisplay = apiKey;
            }

            var possibleKeys = (requirement.ApiKeys.Any() ? string.Join(", ", requirement.ApiKeys.Select(k => k.Substring(0, 4))) : "NO REGISTERED KEYS");

            _logger.LogWarning($"{keyToDisplay}... does not match an entry from {possibleKeys}");

            context.Fail();
        }
    }
}
