using Microsoft.Extensions.Diagnostics.HealthChecks;
using mysb_forms.core;
using mysb_forms.core.Interfaces;
using mysb_forms.core.Models;

namespace mysb_forms.HealthServices;

public class ExternalHealthCheck : IHealthCheck {
    private readonly HttpClient _iPassClient;
    private readonly IPassSetting _ipassSettings;

    public ExternalHealthCheck(ISettings settings, IHttpClientFactory httpClientFactory) {

        _ipassSettings = settings.IPassSettings;
        _iPassClient = httpClientFactory.CreateClient(Constants.IPASS_HTTP_CLIENT);

    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        var customerTokenUrl = $"/customer_forms/v1/token?customer_id={_ipassSettings.HealthCheckCustomerId}";
        var apiResponse = await _iPassClient.GetAsync(customerTokenUrl);
        return apiResponse.IsSuccessStatusCode ? HealthCheckResult.Healthy("iPass Customer Token health check passed") : HealthCheckResult.Unhealthy("iPass Customer Token health failed!!!");
    }
}
