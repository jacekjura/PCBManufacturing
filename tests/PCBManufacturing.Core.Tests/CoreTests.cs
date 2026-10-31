using PCBManufacturing.Core.Catalog;
using PCBManufacturing.Core.Formatting;
using PCBManufacturing.Core.Graphics;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Models;
using PCBManufacturing.Core.Ordering;
using PCBManufacturing.Core.Preferences;
using PCBManufacturing.Core.Pricing;
using PCBManufacturing.Core.Validation;

namespace PCBManufacturing.Core.Tests;

public class CatalogTests
{
    [Fact]
    public void ShippedCatalog_HasMaterialsColorsThicknessesAndOneDefaultPerGroup()
    {
        var catalog = new JsonCatalogService(ContentFiles.CatalogPath(AppContext.BaseDirectory)).Load();

        Assert.True(catalog.BasePricePerBoard > 0);
        Assert.True(catalog.Materials.Count >= 3);
        Assert.True(catalog.SolderMaskColors.Count >= 3);
        Assert.True(catalog.Thicknesses.Count >= 3);
        Assert.Equal(1, catalog.Parameters.Count(parameter => parameter.Kind == ParameterKind.Size && parameter.IsDefault));
        Assert.Equal(1, catalog.Parameters.Count(parameter => parameter.Kind == ParameterKind.Layers && parameter.IsDefault));
        Assert.Equal(1, catalog.Parameters.Count(parameter => parameter.Kind == ParameterKind.Finish && parameter.IsDefault));
        Assert.Equal(1, catalog.Parameters.Count(parameter => parameter.Kind == ParameterKind.Quantity && parameter.IsDefault));
        Assert.Contains(catalog.Materials, material => material.PriceModifier != 1m && material.ProductionDays > 0);
    }

    [Fact]
    public void InvalidCatalog_IsRejected()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        File.WriteAllText(path, """{"basePricePerBoard": 0, "currency": "USD"}""");

        var exception = Assert.Throws<InvalidDataException>(() => new JsonCatalogService(path).Load());
        Assert.Contains("Base price", exception.Message);
    }
}

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
        Material = new MaterialDefinition { Id = "m", NameKey = "Material.Fr4", PriceModifier = materialModifier, ProductionDays = materialDays, SubstrateHex = "#C4A574" },
        Thickness = new BoardThicknessDefinition { Id = "t", NameKey = "Thickness.16", Millimeters = 1.6, PriceModifier = thicknessModifier, ProductionDaysExtra = thicknessDays },
        Size = new ParameterDefinition { Id = "s", Kind = ParameterKind.Size, GroupKey = "Group.Dimensions", NameKey = "Param.Size100", ValueKey = "Param.Size100.Value", PriceModifier = sizeModifier, WidthMm = 100, HeightMm = 80 },
        Layers = new ParameterDefinition { Id = "l", Kind = ParameterKind.Layers, GroupKey = "Group.Layers", NameKey = "Param.Layers2", ValueKey = "Param.Layers2.Value", PriceModifier = layersModifier, LayerCount = 2, ProductionDaysExtra = layersDays },
        Finish = new ParameterDefinition { Id = "f", Kind = ParameterKind.Finish, GroupKey = "Group.Finish", NameKey = "Param.Hasl", ValueKey = "Param.Hasl.Value", PriceModifier = finishModifier, FinishId = "hasl", ProductionDaysExtra = finishDays },
        Quantity = new ParameterDefinition { Id = "q", Kind = ParameterKind.Quantity, GroupKey = "Group.Quantity", NameKey = "Param.Qty10", ValueKey = "Param.Qty10.Value", PriceModifier = quantityModifier, Quantity = quantity }
    };
}

public class PostcodeValidatorTests
{
    private readonly PostcodeValidator _validator = new();

    [Theory]
    [InlineData("94105")]
    [InlineData("94105-1234")]
    [InlineData("SW1A 1AA")]
    [InlineData("sw1a1aa")]
    [InlineData("EC1A 1BB")]
    [InlineData("101000")]
    [InlineData("  GIR 0AA ")]
    public void AcceptsUsUkAndRussianCodes(string postcode) =>
        Assert.True(_validator.Validate(postcode).IsValid);

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("simas")]
    [InlineData("12")]
    [InlineData("ABCDE")]
    [InlineData("9410")]
    public void RejectsEmptyAndMalformedCodes(string postcode)
    {
        var result = _validator.Validate(postcode);
        Assert.False(result.IsValid);
        Assert.Equal(string.IsNullOrWhiteSpace(postcode) ? PostcodeError.Empty : PostcodeError.Invalid, result.Error);
    }
}

public class LocalizationTests
{
    [Fact]
    public void EnglishAndRussian_HaveTheSameKeys()
    {
        var directory = ContentFiles.LocalizationDirectory(AppContext.BaseDirectory);
        var english = File.ReadAllText(Path.Combine(directory, "en.json"));
        var russian = File.ReadAllText(Path.Combine(directory, "ru.json"));
        var en = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(english)!;
        var ru = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(russian)!;

        Assert.Equal(en.Keys.OrderBy(key => key), ru.Keys.OrderBy(key => key));
        Assert.NotEqual("App.Title", en["App.Title"]);
        Assert.NotEqual(en["Tab.Preferences"], ru["Tab.Preferences"]);
    }

    [Fact]
    public void CatalogKeys_ExistInBothLanguages()
    {
        var service = new JsonLocalizationService(ContentFiles.LocalizationDirectory(AppContext.BaseDirectory));
        var catalog = new JsonCatalogService(ContentFiles.CatalogPath(AppContext.BaseDirectory)).Load();
        var keys = catalog.Materials.SelectMany(item => new[] { item.NameKey, item.DescriptionKey })
            .Concat(catalog.SolderMaskColors.Select(item => item.NameKey))
            .Concat(catalog.Thicknesses.Select(item => item.NameKey))
            .Concat(catalog.Parameters.SelectMany(item => new[] { item.NameKey, item.ValueKey, item.GroupKey }))
            .Distinct();

        foreach (var key in keys)
            Assert.NotEqual(key, service[key]);

        service.SetCulture("ru");
        Assert.Equal("ru", service.Culture);
        Assert.Equal("Параметры", service["Tab.Preferences"]);
        Assert.Equal("missing-key", service["missing-key"]);
    }

    [Fact]
    public void CultureChange_RaisesPropertyChanged()
    {
        var service = new JsonLocalizationService(ContentFiles.LocalizationDirectory(AppContext.BaseDirectory));
        var raised = false;
        service.PropertyChanged += (_, _) => raised = true;

        service.SetCulture("ru-RU");

        Assert.True(raised);
        Assert.Contains("дн", service.Format("Summary.Days", 5));
    }
}

public class PreferencesStoreTests
{
    [Fact]
    public async Task RoundTrip_PreservesSelectionsThemeAndCulture()
    {
        var path = Path.Combine(Path.GetTempPath(), "pcb-tests", Guid.NewGuid().ToString(), "preferences.json");
        var store = new JsonPreferencesStore(path);
        var saved = new SavedPreferences
        {
            ProjectName = "AC-9001",
            Postcode = "SW1A 1AA",
            Notes = "Rush",
            MaterialId = "aluminum",
            SolderMaskColorId = "blue",
            ThicknessId = "t08",
            QuantityId = "qty-25",
            SizeId = "size-160",
            LayersId = "layers-4",
            FinishId = "finish-enig",
            Theme = AppTheme.Dark,
            Culture = "ru"
        };

        await store.SaveAsync(saved);
        var loaded = await store.LoadAsync();

        Assert.Equal(saved, loaded);
    }

    [Fact]
    public async Task CorruptFile_ReturnsNullAndKeepsABackup()
    {
        var path = Path.Combine(Path.GetTempPath(), "pcb-tests", Guid.NewGuid().ToString(), "preferences.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, "{not json");

        var loaded = await new JsonPreferencesStore(path).LoadAsync();

        Assert.Null(loaded);
        Assert.True(File.Exists(path + ".invalid"));
    }
}

public class OrderServiceTests
{
    [Fact]
    public async Task Submit_UsesTheClockAndNumberFactory()
    {
        var clock = new FixedClock(new DateTimeOffset(2026, 10, 29, 18, 0, 0, TimeSpan.Zero));
        var service = new OrderService(new FixedNumbers("PCB-20261029-1001"), clock, TimeSpan.Zero);
        var draft = new OrderDraft
        {
            ProjectName = "AC-9001",
            Postcode = "94105",
            Notes = "",
            MaterialKey = "Material.Fr4",
            MaskKey = "Color.Green",
            MaskHex = "#1B7A3A",
            ThicknessKey = "Thickness.16",
            SizeKey = "Param.Size100",
            FinishKey = "Param.Hasl",
            Layers = 2,
            Quantity = 10,
            Total = 250m,
            Currency = "USD",
            ProductionDays = 5
        };

        var confirmation = await service.SubmitAsync(draft);

        Assert.Equal("PCB-20261029-1001", confirmation.Number);
        Assert.Equal(250m, confirmation.Total);
        Assert.Equal(new DateTimeOffset(2026, 10, 29, 18, 0, 0, TimeSpan.Zero), confirmation.CreatedAt);
    }

    [Fact]
    public async Task History_RoundTripsAndCapsTheFile()
    {
        var path = Path.Combine(Path.GetTempPath(), "pcb-tests", Guid.NewGuid().ToString(), "orders.json");
        var store = new JsonOrderHistoryStore(path);
        var orders = Enumerable.Range(0, 60).Select(index => new OrderConfirmation { Number = "PCB-" + index, Total = index }).ToList();

        await store.SaveAsync(orders);
        var loaded = await store.LoadAsync();

        Assert.Equal(JsonOrderHistoryStore.MaxOrders, loaded.Count);
        Assert.Equal("PCB-0", loaded[0].Number);
    }

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class FixedNumbers(string number) : IOrderNumberFactory
    {
        public string Create(DateTimeOffset timestamp) => number;
    }
}

public class ArtworkTests
{
    private readonly PcbArtworkFactory _factory = new();

    [Fact]
    public void Board_UsesMaskColorAndDrawsPadsTracesAndHoles()
    {
        var shapes = _factory.CreateBoard(Request(layers: 2));

        Assert.Contains(shapes, shape => shape.Role == ShapeRole.Mask && shape.FillHex == "#1A4F8B");
        Assert.Contains(shapes, shape => shape.Role == ShapeRole.Pad && shape.FillHex == PcbArtworkFactory.FinishColor("enig"));
        Assert.Contains(shapes, shape => shape.Role == ShapeRole.Trace);
        Assert.True(shapes.Count(shape => shape.Role == ShapeRole.Hole) >= 8);
        Assert.All(shapes, shape => Assert.False(string.IsNullOrWhiteSpace(shape.Geometry)));
    }

    [Fact]
    public void LayerStack_GrowsWithTheLayerCount()
    {
        var two = _factory.CreateLayerStack(Request(layers: 2)).Count(shape => shape.Role is ShapeRole.Mask or ShapeRole.Copper or ShapeRole.Core);
        var six = _factory.CreateLayerStack(Request(layers: 6)).Count(shape => shape.Role is ShapeRole.Mask or ShapeRole.Copper or ShapeRole.Core);

        Assert.Equal(5, two);
        Assert.Equal(13, six);
    }

    [Fact]
    public void IconsAndRoundedRects_ArePathMarkup()
    {
        Assert.Contains("Z", PathMarkup.RoundedRect(0, 0, 100, 40, 8));
        Assert.Contains("A", PathMarkup.Circle(10, 10, 4));
        foreach (var name in IconGeometry.Names)
        {
            var icon = IconGeometry.Get(name);
            Assert.StartsWith("M", icon.Geometry.Trim());
        }
    }

    [Fact]
    public void Money_UsesTheRequestedCurrencySymbol()
    {
        var formatted = Money.Format(12.5m, "USD", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        Assert.Equal("$12.50", formatted);
    }

    private static PcbArtworkRequest Request(int layers) => new()
    {
        WidthMm = 100,
        HeightMm = 80,
        LayerCount = layers,
        ThicknessMm = 1.6,
        MaskHex = "#1A4F8B",
        SubstrateHex = "#C4A574",
        FinishId = "enig"
    };
}
