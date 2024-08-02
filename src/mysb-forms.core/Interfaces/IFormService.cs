namespace mysb_forms.core;

public interface IFormService {
    Task<string> GetFormTemplateAsync(string formId);
    Task<bool> IsFormEnabled(string formId);
}
