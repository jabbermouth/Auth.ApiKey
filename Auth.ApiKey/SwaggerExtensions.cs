using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Auth.ApiKey;

public static class SwaggerExtensions
{
    public static void AddAuthorizeButton(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("API Key Authorisation Scheme", new OpenApiSecurityScheme()
        {
            In = ParameterLocation.Header,
            Name = ApiKeyRequirementHandler.ApikeyHeaderName,
            Type = SecuritySchemeType.ApiKey,
        });

        options.OperationFilter<AuthoriseCheckOperationFilter>();
    }
}
