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

internal sealed class ViewModelGraph
{
    public required PreferencesViewModel Preferences { get; init; }
    public required QuoteViewModel Quote { get; init; }
    public required OrderViewModel Order { get; init; }
    public required MainViewModel Main { get; init; }
    public required FakeDialogs Dialogs { get; init; }
    public required FakeOrders Orders { get; init; }
    public required FakeThemes Themes { get; init; }
    public required JsonOrderHistoryStore History { get; init; }

    public static ViewModelGraph Create(string? directory = null)
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
        return new ViewModelGraph
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

internal sealed class FakeDialogs : IDialogService
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

internal sealed class FakeOrders : IOrderService
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

internal sealed class FakeThemes : IThemeApplier
{
    public List<AppTheme> Applications { get; } = [];
    public void Apply(AppTheme theme) => Applications.Add(theme);
}
