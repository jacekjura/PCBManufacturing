using Microsoft.Extensions.DependencyInjection;
using PCBManufacturing.Core;
using PCBManufacturing.Core.Catalog;
using PCBManufacturing.Core.Graphics;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Ordering;
using PCBManufacturing.Core.Preferences;
using PCBManufacturing.Core.Pricing;
using PCBManufacturing.Core.Validation;

namespace PCBManufacturing.ViewModels;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPcbManufacturing(this IServiceCollection services, string contentRoot, string appDataDirectory)
    {
        ArgumentNullException.ThrowIfNull(services);
        Directory.CreateDirectory(appDataDirectory);

        services.AddSingleton<ICatalogService>(_ => new JsonCatalogService(ContentFiles.CatalogPath(contentRoot)));
        services.AddSingleton<IQuoteCalculator, QuoteCalculator>();
        services.AddSingleton<IPcbArtworkFactory, PcbArtworkFactory>();
        services.AddSingleton<IPostcodeValidator, PostcodeValidator>();
        services.AddSingleton<IPreferencesStore>(_ => new JsonPreferencesStore(Path.Combine(appDataDirectory, "preferences.json")));
        services.AddSingleton<IOrderHistoryStore>(_ => new JsonOrderHistoryStore(Path.Combine(appDataDirectory, "orders.json")));
        services.AddSingleton<IOrderNumberFactory, OrderNumberFactory>();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IOrderService, OrderService>();
        services.AddSingleton<ILocalizationService>(_ => new JsonLocalizationService(ContentFiles.LocalizationDirectory(contentRoot)));
        services.AddSingleton<PreferencesViewModel>();
        services.AddSingleton<QuoteViewModel>();
        services.AddSingleton<OrderViewModel>();
        services.AddSingleton<MainViewModel>();
        return services;
    }
}
