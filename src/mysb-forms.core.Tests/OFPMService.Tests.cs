using EasyCaching.Core;
using mysb_forms.core.Interfaces;
using mysb_forms.core.Models;
using NSubstitute;

namespace mysb_forms.core.Tests;

public class OFPMServiceTests {
    private readonly ISettings _settings =Substitute.For<ISettings>();
    private readonly IHttpClientFactory _httpClientFactory = Substitute.For<IHttpClientFactory>();
    private readonly IEasyCachingProvider _cachingProvider =Substitute.For<IEasyCachingProvider>();
    private readonly IDatabaseService _databaseService=Substitute.For<IDatabaseService>();


    IFormService CreateNewOFPMService(OfpmSetting? ofpmSetting = null) {
        //Arrange
        _settings.OfpmSettings.Returns(ofpmSetting ?? new OfpmSetting() { FormsSettingLookupEnabled = true });
        return new OFPMService(_settings, _httpClientFactory, _cachingProvider, _databaseService);
    }

    [Fact]
    public void Should_Construct_OFPMService() {

        // Act
        var service = CreateNewOFPMService();

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Should_Receive_ISettings() {
        // Arrange
        var service = CreateNewOFPMService();
        // Act & Assert
        _settings.Received();
    }

    [Fact]
    public void Should_Receive_IHttpClientFactory() {
        // Arrange
        var service = CreateNewOFPMService();
        // Act & Assert
        _httpClientFactory.Received();
    }

    [Fact]
    public void Should_Receive_IEasyCachingProvider() {
        // Arrange
        var service = CreateNewOFPMService();
        // Act & Assert
        _cachingProvider.Received();
    }

    [Fact]
    public void Should_Receive_IDatabaseService() {
        // Arrange
        var service = CreateNewOFPMService();
        // Act & Assert
        _databaseService.Received();
    }

    [Fact]
    public async void IsFormEnabled_Given_FormsSettingLookupEnabled_false_Should_Default_Always_Should_Return_True() {
        // Arrange
        var service = CreateNewOFPMService(new OfpmSetting() { FormsSettingLookupEnabled = false });
        // Act
        var actual = await service.IsFormEnabled("waste_report_bin_problem");
        // Assert
        Assert.True(actual);
    }

    [Fact]
    public async void IsFormEnabled_Caching_Null_Should_Return_Enabled_Settings_From_Database() {
        // Arrange
        var service = CreateNewOFPMService();
        bool? cacheValue = false;
        _cachingProvider.GetAsync<bool?>(Arg.Any<string>()).Returns(new CacheValue<bool?>(cacheValue, false));
        _databaseService.GetItemByFormIdAsync("waste_report_bin_problem").Returns(new Item { Enabled = true });
        // Act
        var actual = await service.IsFormEnabled("waste_report_bin_problem");
        // Assert
        Assert.True(actual);
    }

}
