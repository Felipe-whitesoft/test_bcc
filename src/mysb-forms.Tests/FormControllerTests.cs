using FluentAssertions;
using Microsoft.Extensions.Logging;
using mysb_forms.Controllers;
using mysb_forms.core;
using mysb_forms.Models;
using NSubstitute;
namespace mysb_forms.Tests;

public class FormControllerTests {
    private readonly FormController _controller;
    private readonly ILogger<FormController> _logger;

    private readonly IFormService _formService;

    public FormControllerTests() {
        _logger = Substitute.For<ILogger<FormController>>();
        _formService = Substitute.For<IFormService>();
        _controller = new FormController(_logger, _formService);
    }

    [Fact]
    public async void GetTemplate_Given_FormId_Should_Returns_Expected_Result() {
        var actual = await _controller.GetTemplate("waste_report_bin_problem");
        actual.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("123")]
    public async void GetTemplate_Given_FormId_NullOrEmpty_Should_Return_400_Error(string formId) {
        // Arrange
        // Act
        var actual = await _controller.GetTemplate(formId);
        // Assert
        actual.StatusCode.Should().Be(400);
        actual.Content.Should().Be("Form ID is invalid or malformed");
    }


    [Fact]
    public async void GetTemplate_Given_FormId_With_Enabled_false_Should_Return_404_Error() {
        // Arrange
        string formId = "waste_report_bin_problem";
        // Act
        var actual = await _controller.GetTemplate(formId);
        // Assert
        actual.StatusCode.Should().Be(404);
        actual.Content.Should().Be("waste_report_bin_problem form is not available");
    }

    [Fact]
    public async void GetTemplate_Given_FormId_Should_Returns_Form_template() {
        var expectedContent = "<template >Form Template </template>";
        // Arrange
        _formService.IsFormEnabled(Arg.Any<string>()).Returns(true);
        _formService.GetFormTemplateAsync("waste_report_bin_problem").Returns(expectedContent);

        //Act
        var actual = await _controller.GetTemplate("waste_report_bin_problem");

        //Assert
        actual.Content.Should().Be(expectedContent);
    }

}
