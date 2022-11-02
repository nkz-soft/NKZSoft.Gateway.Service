namespace NKZSoft.Gateway.API.Extensions;

public static class WebApplicationExtension
{
    public static void UseGatewayEndpoints(this IEndpointRouteBuilder app, string? redirectUrl)
    {
        app.UseUserInfoEndpoint();
        app.UseLoginEndpoint(redirectUrl);
        app.UseLogoutEndpoint(redirectUrl);
    }

    private static void UseLogoutEndpoint(this IEndpointRouteBuilder app, string? url) =>
        app.MapGet("/logout", (string? redirectUrl, HttpContext ctx) =>
        {
            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = url + "/userinfo";
            }

            var authProps = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            var authSchemes = new[] {
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme
            };

            return Results.SignOut(authProps, authSchemes);
        });

    private static void UseLoginEndpoint(this IEndpointRouteBuilder app, string? url) =>
        app.MapGet("/login", async (string? redirectUrl, HttpContext ctx) =>
        {

            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = url + "/userinfo";
            }

            await ctx.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            });
        });

    private static void UseUserInfoEndpoint(this IEndpointRouteBuilder app) =>
        app.MapGet("/userinfo", (ClaimsPrincipal user) =>
        {
            var claims = user.Claims;
            var dict = new Dictionary<string, string>();

            foreach (var entry in claims)
            {
                dict[entry.Type] = entry.Value;
            }

            return dict;
        });
}
