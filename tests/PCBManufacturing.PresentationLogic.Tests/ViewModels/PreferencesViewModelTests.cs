using PCBManufacturing.Core;
using PCBManufacturing.Core.Catalog;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Validation;
using PCBManufacturing.ViewModels;

namespace PCBManufacturing.ViewModels.Tests;

public class PreferencesViewModelTests
{
    [Fact]
    public void MaterialSelection_UpdatesPriceModifierAndLeadTime()
    {
        var preferences = CreatePreferences();

        preferences.SelectedMaterial = preferences.Materials.Single(option => option.Id == "aluminum");

        Assert.Equal("×1.45", preferences.SummaryModifier);
        Assert.Equal("8 days", preferences.SummaryLead);
        Assert.Contains("Aluminum", preferences.SummaryMaterial);
    }

    [Fact]
    public void Postcode_ShowsAndClearsTheInvalidState()
    {
        var preferences = CreatePreferences();

        preferences.Postcode = "simas";

        Assert.False(preferences.IsPostcodeValid);
        Assert.True(preferences.HasErrors);
        Assert.True(preferences.HasPostcodeError);
        Assert.NotEmpty(preferences.PostcodeMessage);

        preferences.Postcode = "SW1A 1AA";

        Assert.True(preferences.IsPostcodeValid);
        Assert.False(preferences.HasPostcodeError);
        Assert.Equal("Valid", preferences.SummaryPostcodeState);
    }

    [Fact]
    public void MaskSelection_ExposesTheHexColorForThePreview()
    {
        var preferences = CreatePreferences();

        preferences.SelectedSolderMask = preferences.SolderMaskColors.Single(option => option.Id == "red");

        Assert.Equal("#B42318", preferences.MaskHex);
    }

    private static PreferencesViewModel CreatePreferences()
    {
        var root = AppContext.BaseDirectory;
        return new PreferencesViewModel(
            new JsonCatalogService(ContentFiles.CatalogPath(root)),
            new PostcodeValidator(),
            new JsonLocalizationService(ContentFiles.LocalizationDirectory(root)));
    }
}
