using PCBManufacturing.Core;
using PCBManufacturing.Core.Services.Catalog;
using PCBManufacturing.Core.Graphics;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Services.Ordering;
using PCBManufacturing.Core.Services.Preferences;
using PCBManufacturing.Core.Services.Pricing;
using PCBManufacturing.Core.Services.Validation;
using PCBManufacturing.PresentationLogic.ViewModels;

namespace PCBManufacturing.PresentationLogic.Tests.ViewModels;

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
