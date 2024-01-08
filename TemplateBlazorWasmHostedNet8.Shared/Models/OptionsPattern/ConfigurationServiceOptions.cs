namespace TemplateBlazorWasmHostedNet8.Shared.Models.OptionsPattern;

public class ConfigurationServiceOptions
{
    public string ApiKeyJwt { get; set; }
    public int TokenJwtExpiresInMinutes { get; set; }
    public string UrlClient { get; set; }
}