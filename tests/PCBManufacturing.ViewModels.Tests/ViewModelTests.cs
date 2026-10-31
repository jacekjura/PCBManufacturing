using PCBManufacturing.Core;
using PCBManufacturing.Core.Catalog;
using PCBManufacturing.Core.Dialogs;
using PCBManufacturing.Core.Graphics;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Models;
using PCBManufacturing.Core.Ordering;
using PCBManufacturing.Core.Preferences;
using PCBManufacturing.Core.Pricing;
using PCBManufacturing.Core.Theming;
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

public class OrderAndShellTests
{
    [Fact]
    public async Task InvalidPostcode_OpensTheValidationDialogAndDoesNotSubmit()
    {
        var graph = Graph.Create();
        graph.Preferences.Postcode = "simas";

        await graph.Order.PlaceOrderCommand.ExecuteAsync(null);

        Assert.Empty(graph.Orders.Submitted);
        Assert.NotEmpty(graph.Dialogs.Alerts);
        Assert.Empty(graph.Dialogs.Confirmations);
        Assert.Contains(graph.Order.ValidationMessages, message => message.Contains("postcode", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ConfirmingAValidOrder_SubmitsAndRecordsHistory()
    {
        var graph = Graph.Create();
        graph.Preferences.Postcode = "94105";

        await graph.Order.PlaceOrderCommand.ExecuteAsync(null);

        Assert.Single(graph.Dialogs.Confirmations);
        Assert.Single(graph.Orders.Submitted);
        Assert.Single(graph.Order.History);
        Assert.Contains("PCB-TEST", graph.Order.StatusMessage);
        var stored = await graph.History.LoadAsync();
        Assert.Single(stored);
    }

    [Fact]
    public async Task DecliningTheDialog_DoesNotSubmit()
    {
        var graph = Graph.Create();
        graph.Preferences.Postcode = "101000";
        graph.Dialogs.ConfirmResult = false;

        await graph.Order.PlaceOrderCommand.ExecuteAsync(null);

        Assert.Empty(graph.Orders.Submitted);
        Assert.Empty(graph.Order.History);
    }

    [Fact]
    public async Task SaveAndReload_RestoresThemeLanguageAndSelections()
    {
        var directory = Path.Combine(Path.GetTempPath(), "pcb-tests", Guid.NewGuid().ToString());
        var first = Graph.Create(directory);
        first.Preferences.Postcode = "EC1A 1BB";
        first.Preferences.ProjectName = "Sensor";
        first.Preferences.SelectedMaterial = first.Preferences.Materials.Single(option => option.Id == "aluminum");
        first.Main.ToggleThemeCommand.Execute(null);
        first.Main.IsRussian = true;
        first.Quote.Groups.Single(group => group.Kind == ParameterKind.Finish).SelectedRow =
            first.Quote.Groups.Single(group => group.Kind == ParameterKind.Finish).Rows.Single(row => row.Definition.Id == "finish-enig");

        await first.Main.SaveCommand.ExecuteAsync(null);

        var second = Graph.Create(directory);
        await second.Main.InitializeAsync();

        Assert.Equal("Sensor", second.Preferences.ProjectName);
        Assert.Equal("EC1A 1BB", second.Preferences.Postcode);
        Assert.Equal("aluminum", second.Preferences.SelectedMaterial?.Id);
        Assert.Equal("finish-enig", second.Quote.SelectedFinish?.Id);
        Assert.Equal(AppTheme.Dark, second.Main.Theme);
        Assert.Equal("ru", second.Main.Culture);
        Assert.Contains(AppTheme.Dark, second.Themes.Applications);
    }

    [Fact]
    public void Discard_RestoresCatalogDefaultsWithoutChangingTheme()
    {
        var graph = Graph.Create();
        graph.Main.ToggleThemeCommand.Execute(null);
        graph.Preferences.Postcode = "94105";
        graph.Preferences.ProjectName = "Temp";
        graph.Quote.Select("size-50", "layers-8", "finish-osp");

        graph.Main.DiscardCommand.Execute(null);

        Assert.Equal("AC-9001", graph.Preferences.ProjectName);
        Assert.Equal("", graph.Preferences.Postcode);
        Assert.Equal("fr4", graph.Preferences.SelectedMaterial?.Id);
        Assert.Equal("size-100", graph.Quote.SelectedSize?.Id);
        Assert.Equal(AppTheme.Dark, graph.Main.Theme);
    }

    [Fact]
    public async Task PlaceOrderCommand_IsAsyncAndBusyStateRejectsASecondSubmit()
    {
        var graph = Graph.Create();
        graph.Preferences.Postcode = "94105";
        graph.Orders.Delay = TimeSpan.FromMilliseconds(200);

        var running = graph.Order.PlaceOrderCommand.ExecuteAsync(null);
        Assert.True(graph.Order.IsBusy);
        Assert.False(graph.Order.PlaceOrderCommand.CanExecute(null));
        await running;
        Assert.False(graph.Order.IsBusy);
        Assert.True(graph.Order.PlaceOrderCommand.CanExecute(null));
    }

    private sealed class Graph
    {
        public required PreferencesViewModel Preferences { get; init; }
        public required QuoteViewModel Quote { get; init; }
        public required OrderViewModel Order { get; init; }
        public required MainViewModel Main { get; init; }
        public required FakeDialogs Dialogs { get; init; }
        public required FakeOrders Orders { get; init; }
        public required FakeThemes Themes { get; init; }
        public required JsonOrderHistoryStore History { get; init; }

        public static Graph Create(string? directory = null)
        {
            directory ??= Path.Combine(Path.GetTempPath(), "pcb-tests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory);
            var root = AppContext.BaseDirectory;
            var text = new JsonLocalizationService(ContentFiles.LocalizationDirectory(root));
            var catalog = new JsonCatalogService(ContentFiles.CatalogPath(root));
            var preferences = new PreferencesViewModel(catalog, new PostcodeValidator(), text);
            var quote = new QuoteViewModel(preferences, new QuoteCalculator(), new PcbArtworkFactory(), text);
            var dialogs = new FakeDialogs();
            var orders = new FakeOrders();
            var history = new JsonOrderHistoryStore(Path.Combine(directory, "orders.json"));
            var order = new OrderViewModel(preferences, quote, orders, history, dialogs, text);
            var themes = new FakeThemes();
            var main = new MainViewModel(
                preferences,
                quote,
                order,
                new JsonPreferencesStore(Path.Combine(directory, "preferences.json")),
                history,
                text,
                themes,
                catalog);
            return new Graph
            {
                Preferences = preferences,
                Quote = quote,
                Order = order,
                Main = main,
                Dialogs = dialogs,
                Orders = orders,
                Themes = themes,
                History = history
            };
        }
    }

    private sealed class FakeDialogs : IDialogService
    {
        public bool ConfirmResult { get; set; } = true;
        public List<DialogContent> Confirmations { get; } = [];
        public List<DialogContent> Alerts { get; } = [];

        public Task<bool> ConfirmAsync(DialogContent content, CancellationToken cancellationToken = default)
        {
            Confirmations.Add(content);
            return Task.FromResult(ConfirmResult);
        }

        public Task AlertAsync(DialogContent content, CancellationToken cancellationToken = default)
        {
            Alerts.Add(content);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeOrders : IOrderService
    {
        public List<OrderDraft> Submitted { get; } = [];
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;

        public async Task<OrderConfirmation> SubmitAsync(OrderDraft draft, CancellationToken cancellationToken = default)
        {
            Submitted.Add(draft);
            if (Delay > TimeSpan.Zero)
                await Task.Delay(Delay, cancellationToken);
            return new OrderConfirmation
            {
                Number = "PCB-TEST",
                CreatedAt = DateTimeOffset.UnixEpoch,
                ProjectName = draft.ProjectName,
                Postcode = draft.Postcode,
                Total = draft.Total,
                Currency = draft.Currency,
                Quantity = draft.Quantity,
                ProductionDays = draft.ProductionDays,
                MaterialKey = draft.MaterialKey,
                MaskKey = draft.MaskKey,
                MaskHex = draft.MaskHex,
                ThicknessKey = draft.ThicknessKey,
                SizeKey = draft.SizeKey,
                FinishKey = draft.FinishKey,
                Layers = draft.Layers
            };
        }
    }

    private sealed class FakeThemes : IThemeApplier
    {
        public List<AppTheme> Applications { get; } = [];
        public void Apply(AppTheme theme) => Applications.Add(theme);
    }
}
