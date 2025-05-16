namespace Cox.Cmr.Payment.Api.Settings;

[ExcludeFromCodeCoverage]
public class ApplicationSettings
{
    public string Name { get; init; } = string.Empty;
    public string CookieDomain { get; init; } = string.Empty;
    public string RequestHost { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
}
