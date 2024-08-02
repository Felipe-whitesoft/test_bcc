
using System.Text;
using EasyCaching.Core;
using mysb_forms.core.Interfaces;
using mysb_forms.core.Models;

namespace mysb_forms.core;

public class OFPMService : IFormService {
    private const string HEADER_CACHE_KEY = "Ofpm_headers";
    private const string BODY_CACHE_KEY = "Ofpm_body";
    private const string FORM_ENABLED_CACHE_KEY = "form_enabled";


    private readonly IEasyCachingProvider _cacheProvider;
    private readonly IDatabaseService _databaseService;
    private readonly HttpClient _formClient;
    private readonly OfpmSetting _settings;

    public OFPMService(ISettings settings, IHttpClientFactory httpClientFactory, IEasyCachingProvider cacheProvider, IDatabaseService databaseService) {
        _settings = settings.OfpmSettings;
        _formClient = httpClientFactory.CreateClient(Constants.OFPM_FORM_HTTP_CLIENT);
        _cacheProvider = cacheProvider;
        _databaseService = databaseService;
    }

    private async Task<string> GetHeaderContentAsync() {
        //check caching
        if (await _cacheProvider.ExistsAsync(HEADER_CACHE_KEY)) {
            var cacheResult = await _cacheProvider.GetAsync<string>(HEADER_CACHE_KEY);
            return cacheResult.Value;
        }

        //fetch latest content
        var response = await _formClient.GetAsync("/form/resources");
        var formHeaderFragment = await response.Content.ReadAsStringAsync();

        //set caching
        await _cacheProvider.SetAsync(HEADER_CACHE_KEY, formHeaderFragment, TimeSpan.FromMinutes(_settings.ResourceCachingTtlInMinutes));

        return formHeaderFragment;
    }

    private async Task<string> GetUnauthenticatedFormContent(string formId) {
        //check caching
        var formCacheKey = $"{BODY_CACHE_KEY}_{formId}";
        if (await _cacheProvider.ExistsAsync(formCacheKey)) {
            var cacheResult = await _cacheProvider.GetAsync<string>(formCacheKey);
            return cacheResult.Value;
        }

        //fetch latest content
        var response = await _formClient.GetAsync($"/form/widget/auto/{formId}");
        var formBodyFragment = await response.Content.ReadAsStringAsync();

        //set caching
        await _cacheProvider.SetAsync(formCacheKey, formBodyFragment, TimeSpan.FromMinutes(_settings.ContentCachingTtlInMinutes));

        return formBodyFragment;
    }

    public async Task<string> GetFormTemplateAsync(string formId) {
        var headerContent = await GetHeaderContentAsync();
        var formContent = await GetUnauthenticatedFormContent(formId);

        var sbTemplate = new StringBuilder(1024);
        //header
        sbTemplate.AppendFormat("<template data-id='form-template-header' data-form-id='{0}'>", formId);
        sbTemplate.AppendLine(headerContent);
        sbTemplate.AppendLine("</template>");

        //body
        sbTemplate.AppendFormat("<template data-id='form-template-body' data-form-id='{0}'>", formId);
        sbTemplate.AppendLine(formContent);
        sbTemplate.AppendLine("</template>");
        return sbTemplate.ToString();
    }

    public async Task<bool> IsFormEnabled(string formId) {

        //check if setting lookup is enabled then only proceed
        if (!_settings.FormsSettingLookupEnabled) {
            //default make all forms enabled
            return true;
        }

        //check caching
        var formEnabledCacheKey = $"{FORM_ENABLED_CACHE_KEY}_{formId}";
        var formItem = await _cacheProvider.GetAsync<Item>(formEnabledCacheKey);
        if (formItem != null && formItem.HasValue) {
            return formItem.Value.Enabled ?? false;
        }

        //fetch latest setting
        var item = await _databaseService.GetItemByFormIdAsync(formId);
        //set caching
        await _cacheProvider.SetAsync(formEnabledCacheKey, item, TimeSpan.FromMinutes(_settings.FormsSettingCachingTtlInMinutes));
        return item.Enabled ?? false;
    }
}
