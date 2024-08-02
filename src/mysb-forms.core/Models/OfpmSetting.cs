namespace mysb_forms.core.Models;

public class OfpmSetting {
    public string BaseUrl { get; set; } = string.Empty;
    public bool FormsSettingLookupEnabled { get; set; }
    public double ResourceCachingTtlInMinutes { get; set; }
    public double ContentCachingTtlInMinutes { get; set; }
    public double FormsSettingCachingTtlInMinutes { get; set; }
}

