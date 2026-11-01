using Microsoft.Extensions.DependencyInjection;
using PCBManufacturing.Core;
using PCBManufacturing.Core.Services.Catalog;
using PCBManufacturing.Core.Graphics;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Services.Ordering;
using PCBManufacturing.Core.Services.Preferences;
using PCBManufacturing.Core.Services.Pricing;
using PCBManufacturing.Core.Services.Validation;
using PCBManufacturing.PresentationLogic.ViewModels;

namespace PCBManufacturing.PresentationLogic;

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
