namespace mysb_forms.Models;

public class CorsPolicy {
    public IList<string>? Origins { get; set; }
    public bool AllowAnyOrigin { get; set; }
}

