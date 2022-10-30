using NKZSoft.Gateway.API.Extensions;
using OpenIdConnectConfiguration = NKZSoft.Gateway.API.Settings.OpenIdConnectConfiguration;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddCommandLine(args)
    .AddEnvironmentVariables()
    .Build();

var configuration = builder.Configuration;

var openIdConnectConfiguration = configuration.GetSection(OpenIdConnectConfiguration.SectionName)
    .Get<OpenIdConnectConfiguration>();

ArgumentNullException.ThrowIfNull(openIdConnectConfiguration);

var services = builder.Services;
services.AddLogging()
    .AddReverseProxy()
    .LoadFromConfig(configuration.GetSection("ReverseProxy"));

services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(options =>
    {
        options.SignInScheme = "Cookies";
        options.Authority = openIdConnectConfiguration.Authority;
        options.ClientId = openIdConnectConfiguration.ClientId;
        options.ClientSecret = openIdConnectConfiguration.ClientSecret;
        options.MetadataAddress = openIdConnectConfiguration.MetadataAddress;
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.UsePkce = true;
        options.Scope.Add("profile");
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.RequireHttpsMetadata = false;
    });

services.AddAuthorization(options => options.AddPolicy("CookieAuthenticationPolicy", configurePolicy =>
{
    configurePolicy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
    configurePolicy.RequireAuthenticatedUser();
}));

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.UseGatewayEndpoints();
    endpoints.MapReverseProxy();
});
app.Run();
