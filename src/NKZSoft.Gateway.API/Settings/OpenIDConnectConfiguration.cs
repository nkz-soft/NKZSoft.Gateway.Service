namespace NKZSoft.Gateway.API.Settings;

internal sealed record OpenIdConnectConfiguration
{
    public const string SectionName = "OpenIDConnect";

    public string? Authority { get; set; }

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }

    public string? MetadataAddress { get; set; }

    public string? RedirectUrl { get; set; }
}
