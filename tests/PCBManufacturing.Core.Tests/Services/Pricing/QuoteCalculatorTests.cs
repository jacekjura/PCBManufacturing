using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Localization.Keys;
using PCBManufacturing.Core.Models.Catalog;
using PCBManufacturing.Core.Models.Quote;
using PCBManufacturing.Core.Services.Pricing;

namespace PCBManufacturing.Core.Tests.Services.Pricing;

public class QuoteCalculatorTests
{
    private readonly QuoteCalculator _calculator = new();

    [Fact]
    public void DefaultModifiers_UseBasePriceTimesQuantity()
    {
        var result = _calculator.Calculate(Sample(materialModifier: 1m, materialDays: 5, quantity: 10, quantityModifier: 1m));

        Assert.Equal(25m, result.UnitPrice);
        Assert.Equal(250m, result.Total);
        Assert.Equal(5, result.ProductionDays);
        Assert.Equal(25m, result.Lines[0].Amount);
        Assert.All(result.Lines.Skip(1), line => Assert.Equal(0m, line.Amount));
    }

    [Fact]
    public void StackedModifiers_RoundTheUnitPriceAndScaleByQuantity()
    {
        var result = _calculator.Calculate(Sample(
            materialModifier: 1.45m,
            materialDays: 8,
            sizeModifier: 0.70m,
            thicknessModifier: 1.10m,
            thicknessDays: 1,
            layersModifier: 1.65m,
            layersDays: 2,
            finishModifier: 1.40m,
            finishDays: 1,
            quantity: 5,
            quantityModifier: 1.15m));

        Assert.Equal(74.15m, result.UnitPrice);
        Assert.Equal(370.75m, result.Total);
        Assert.Equal(12, result.ProductionDays);
        Assert.Equal(25.00m, result.Lines[0].Amount);
        Assert.Equal(-7.50m, result.Lines[1].Amount);
        Assert.Equal(7.88m, result.Lines[2].Amount);
        Assert.Equal(2.54m, result.Lines[3].Amount);
        Assert.Equal(18.14m, result.Lines[4].Amount);
        Assert.Equal(18.42m, result.Lines[5].Amount);
        Assert.Equal(9.67m, result.Lines[6].Amount);
        Assert.Equal(result.UnitPrice, result.Lines.Sum(line => line.Amount));
    }

    private static QuoteRequest Sample(
        decimal materialModifier,
        int materialDays,
        int quantity,
        decimal quantityModifier,
        decimal sizeModifier = 1m,
        decimal thicknessModifier = 1m,
        int thicknessDays = 0,
        decimal layersModifier = 1m,
        int layersDays = 0,
        decimal finishModifier = 1m,
        int finishDays = 0) => new()
    {
        BasePricePerBoard = 25m,
        Currency = "USD",
        Material = new MaterialDefinition { Id = "m", NameKey = Material.Fr4, PriceModifier = materialModifier, ProductionDays = materialDays, SubstrateHex = "#C4A574" },
        Thickness = new BoardThicknessDefinition { Id = "t", NameKey = Thickness._16, Millimeters = 1.6, PriceModifier = thicknessModifier, ProductionDaysExtra = thicknessDays },
        Size = new ParameterDefinition { Id = "s", Kind = ParameterKind.Size, GroupKey = Group.Dimensions, NameKey = Param.Size100, ValueKey = Param.Size100Value, PriceModifier = sizeModifier, WidthMm = 100, HeightMm = 80 },
        Layers = new ParameterDefinition { Id = "l", Kind = ParameterKind.Layers, GroupKey = Group.Layers, NameKey = Param.Layers2, ValueKey = Param.Layers2Value, PriceModifier = layersModifier, LayerCount = 2, ProductionDaysExtra = layersDays },
        Finish = new ParameterDefinition { Id = "f", Kind = ParameterKind.Finish, GroupKey = Group.Finish, NameKey = Param.Hasl, ValueKey = Param.HaslValue, PriceModifier = finishModifier, FinishId = "hasl", ProductionDaysExtra = finishDays },
        Quantity = new ParameterDefinition { Id = "q", Kind = ParameterKind.Quantity, GroupKey = Group.Quantity, NameKey = Param.Qty10, ValueKey = Param.Qty10Value, PriceModifier = quantityModifier, Quantity = quantity }
    };
}
