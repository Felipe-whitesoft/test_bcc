using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using mysb_forms.core;

namespace mysb_forms.Controllers;

[ApiController]

[Route("v1/settings")]
public class SettingsController : ControllerBase {

    private readonly ILogger<FormController> _logger;
    private readonly IDatabaseService _databaseService;

    public SettingsController(ILogger<FormController> logger, IDatabaseService databaseService) {
        _logger = logger;
        _databaseService = databaseService;
    }

    /// <summary>
    /// Get Forms template Settings by form ID
    /// </summary>
    /// <param name="formId"></param>
    /// <returns></returns>
    [HttpGet("template/{formId}")]
    [ProducesResponseType(typeof(Item), StatusCodes.Status200OK)]
    public async Task<Item> GetFormSetting([FromRoute] string formId) {
        var identifier = formId?.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(identifier) || identifier.Length < 4) {
            return new Item() {
                Enabled = false
            };
        }

        return await _databaseService.GetItemByFormIdAsync(identifier);
    }
}
