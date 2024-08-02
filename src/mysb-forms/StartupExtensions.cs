using Microsoft.AspNetCore.Cors;
using mysb_forms.core;
using mysb_forms.core.Models;
using mysb_forms.Models;

namespace mysb_forms;

public static class StartupExtensions {
    public static void AddAppSettings(this IServiceCollection services, ConfigurationManager configManager) {
        services.Configure<IPassSetting>(configManager.GetSection("iPass"));
        services.Configure<OfpmSetting>(configManager.GetSection("Ofpm"));
        services.Configure<CosmosDBConfiguration>(configManager.GetSection("CosmosDb"));
    }

    public static void AddHttpClients(this IServiceCollection services, ConfigurationManager configManager) {

        //configure Form / OFPM Http Client
        var formSettings = configManager.GetSection("Ofpm").Get<OfpmSetting>() ?? new OfpmSetting();
        services.AddHttpClient(core.Constants.OFPM_FORM_HTTP_CLIENT, (provider, httpClient) => {
            httpClient.BaseAddress = new Uri(formSettings.BaseUrl);
        });

        //Configure iPaaS Http Clients
        var iPassSettings = configManager.GetSection("iPass").Get<IPassSetting>() ?? new IPassSetting();

        services.AddHttpClient(core.Constants.IPASS_HTTP_CLIENT, (provider, httpClient) => {
            httpClient.BaseAddress = new Uri(iPassSettings.BaseUrl);
        });

        services.AddHttpClient(core.Constants.IPASS_HTTP_OAUTH_CLIENT, (provider, httpClient) => {
            httpClient.BaseAddress = new Uri(iPassSettings.OAuthTokenUrl);
        });

    }

    public static void AddAppInsights(this IServiceCollection services) {
        var aiOptions = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions();
        // Disables adaptive sampling.
        aiOptions.EnableAdaptiveSampling = false;
        // Disables QuickPulse (Live Metrics stream).
        aiOptions.EnableQuickPulseMetricStream = false;
        services.AddApplicationInsightsTelemetry(aiOptions);
    }

    public static void AddCaching(this IServiceCollection services, IConfiguration configuration) {
        services.AddEasyCaching(options => {
            options.UseInMemory("mysb-forms-api-default");
        });
    }

    public static void CreateCorsPolicies(this IServiceCollection services, IConfiguration configuration) {
        var policy = configuration.GetSection("Cors").Get<CorsPolicy>();
        policy = policy ?? new CorsPolicy() {
            AllowAnyOrigin = true,
            Origins = new List<string>()
        };

        if (policy.AllowAnyOrigin) {
            services.AddCors(options => {
                options.AddPolicy("AllowGet", builder => {
                    builder.AllowAnyOrigin()
                        .WithMethods("GET")
                        .AllowAnyHeader();
                });
            });
            return;
        }

        if (policy.Origins == null || policy.Origins.Count == 0) {
            return;
        }

        services.AddCors(options => {
            options.AddPolicy("AllowGet", builder => {
                builder.WithOrigins(policy.Origins.ToArray())
                    .WithMethods("GET")
                    .AllowAnyHeader();
            });
        });

    }
}
