using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using mysb_forms.core;

namespace mysb_forms.Controllers;

[ApiController]

[Route("v1/forms")]
public class FormController : ControllerBase {

    private readonly ILogger<FormController> _logger;
    private readonly IFormService _formService;

    public FormController(ILogger<FormController> logger, IFormService formService) {
        _logger = logger;
        _formService = formService;
    }

    /// <summary>
    /// Retrieves the template for a given form ID.
    /// </summary>
    /// <param name="formId">The unique identifier of the form.</param>
    /// <returns>The template content as a ContentResult.</returns>
    /// <response code="200">Returns the template content.</response>
    /// <response code="400">If the provided form ID is invalid or malformed.</response>
    /// <response code="404">If the form with the given form ID is not available / enabled.</response>
    [EnableCors("AllowGet")]
    [HttpGet("template/{formId}")]
    [ProducesResponseType(typeof(ContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ContentResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ContentResult), StatusCodes.Status404NotFound)]
    public async Task<ContentResult> GetTemplate([FromRoute] string? formId) {
        var identifier = formId?.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(identifier) || identifier.Length < 4) {
            return new ContentResult {
                StatusCode = StatusCodes.Status400BadRequest,
                Content = "Form ID is invalid or malformed",
                ContentType = "text/html"
            };
        }

        var isFormEnabled = await _formService.IsFormEnabled(identifier);
        if (!isFormEnabled) {
            return new ContentResult {
                StatusCode = StatusCodes.Status404NotFound,
                Content = $"{identifier} form is not available",
                ContentType = "text/html"
            };
        }

        var formResult = await _formService.GetFormTemplateAsync(identifier);
        return new ContentResult {
            StatusCode = StatusCodes.Status200OK,
            Content = formResult,
            ContentType = "text/html"
        };
    }
}
