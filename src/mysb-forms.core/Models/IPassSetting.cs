namespace mysb_forms.core.Models;

public class IPassSetting {
    public string BaseUrl { get; set; } = string.Empty;
    public string OAuthTokenUrl { get; set; } = string.Empty;
    public string OAuthScope { get; set; } = string.Empty;
    public string OAuthClientId { get; set; } = string.Empty;
    public string OAuthClientSecret { get; set; } = string.Empty;
    public string OAuthGrantType { get; set; } = string.Empty;
    public string HealthCheckCustomerId { get; set; } = string.Empty;
}

