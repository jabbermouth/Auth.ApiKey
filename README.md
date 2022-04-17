# Overview
This package contains code and extension methods to make the process of API key authentication to an API, including to yor Swagger definition, straightforward by removing a lot of the boilerplate code.

## Setup
### SwaggerGen
Within the `AddSwaggerGen` options, add `config.AddAuthorizeButton();` where `config` is the SwaggerGenOptions variable you have declared.

### Authentication
Assuming the only authentication method being used is API key, the following snippet can be used:
```
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
    config.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
}).AddApiKeySupport();
```

### Authorization
The basic authorization policy (i.e. any key is valid for any thing), can be configured using the following snippet:
```
builder.Services.AddAuthorization(config =>
{
    config.AddApiPolicy(builder.Configuration);
});
```

By default, this looks for a config section called ApiKeys but by passing an addition parameter to `AddApiPolicy`, a difference configuration key can be specified.

### Adding Middleware
Before mapping endpoints, add the authentication and authorization middleware:
```
app.UseAuthentication();
app.UseAuthorization();
```

### appsettings.json
Finally, add a section to appsettings.json (or another file, User Secrets, etc...) using the following format for each API key you wish to permit access:
```
{
  "ApiKeys": [
    "MyFirstApiKey",
    "AnotherApiKey"
  ]
}
```

## Restricting Endpoints
### Controllers
Add the `[Authorize]` attribute before the controller or endpoint you wish to protect.

### Minimal APIs
Either add the `[Authorize]` attribute before the `()` of the handler declaration:
```
app.MapGet("/hellosecureworld", [Authorize] () =>
{
    return "Hello secure world!";
})
```

Alternative, after the declaration, add a `RequireAuthorization()` call after declaring the endpoint:
```
app.MapGet("/hellosecureworld", () =>
{
    return "Hello secure world!";
}).RequireAuthorization();
```
