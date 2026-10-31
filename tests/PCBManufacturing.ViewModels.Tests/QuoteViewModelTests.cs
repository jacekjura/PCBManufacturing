using PCBManufacturing.Core;
using PCBManufacturing.Core.Catalog;
using PCBManufacturing.Core.Graphics;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Models;
using PCBManufacturing.Core.Pricing;
using PCBManufacturing.Core.Validation;
using PCBManufacturing.ViewModels;

namespace PCBManufacturing.ViewModels.Tests;

public class QuoteViewModelTests
{
    [Fact]
    public void Defaults_PriceTenStandardBoardsAtTheBaseRate()
    {
        var (preferences, quote, _) = Create();

        Assert.True(quote.HasQuote);
        Assert.Equal("$25.00", quote.UnitText);
        Assert.Equal("$250.00", quote.TotalText);
        Assert.Equal("5 days", quote.LeadText);
        Assert.Contains(quote.BoardShapes, shape => shape.Role == ShapeRole.Mask && shape.FillHex == preferences.MaskHex);
    }

    [Fact]
    public void MaterialAndMaskChanges_UpdateTheTotalAndThePreview()
    {
        var (preferences, quote, _) = Create();
        var before = quote.PreviewRevision;

        preferences.SelectedMaterial = preferences.Materials.Single(option => option.Id == "rogers");
        preferences.SelectedSolderMask = preferences.SolderMaskColors.Single(option => option.Id == "white");

        Assert.NotEqual("$250.00", quote.TotalText);
        Assert.True(quote.PreviewRevision > before);
        Assert.Contains(quote.BoardShapes, shape => shape.FillHex == "#F4F1EA");
        Assert.True(quote.LayerShapes.Count(shape => shape.Role == ShapeRole.Copper) >= 2);
    }

    [Fact]
    public void SelectingAParameter_ClearsTheOtherRowsInThatGroup()
    {
        var (_, quote, _) = Create();
        var layers = quote.Groups.Single(group => group.Kind == ParameterKind.Layers);
        var six = layers.Rows.Single(row => row.Definition.Id == "layers-6");

        layers.SelectedRow = six;

        Assert.Equal(six, layers.SelectedRow);
        Assert.Equal(1, layers.Rows.Count(row => row.IsSelected));
        Assert.Equal(6, quote.SelectedLayers?.LayerCount);
    }

    [Fact]
    public void CultureChange_RelabelsTheParameterRows()
    {
        var (_, quote, text) = Create();

        text.SetCulture("ru");

        var size = quote.Groups.Single(group => group.Kind == ParameterKind.Size);
        Assert.Equal("Размеры", size.Title);
        Assert.Contains(size.Rows, row => row.Value.Contains("мм", StringComparison.Ordinal));
    }

    private static (PreferencesViewModel Preferences, QuoteViewModel Quote, JsonLocalizationService Text) Create()
    {
        var text = new JsonLocalizationService(ContentFiles.LocalizationDirectory(AppContext.BaseDirectory));
        var preferences = new PreferencesViewModel(
            new JsonCatalogService(ContentFiles.CatalogPath(AppContext.BaseDirectory)),
            new PostcodeValidator(),
            text);
        var quote = new QuoteViewModel(preferences, new QuoteCalculator(), new PcbArtworkFactory(), text);
        return (preferences, quote, text);
    }
}
