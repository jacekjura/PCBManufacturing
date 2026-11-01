using System.Text.Json;
using PCBManufacturing.Core;
using PCBManufacturing.Core.Services.Catalog;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Localization.Keys;

namespace PCBManufacturing.Core.Tests.Localization;

public class JsonLocalizationServiceTests
{
    [Fact]
    public void EnglishAndRussian_HaveTheSameKeys()
    {
        var directory = ContentFiles.LocalizationDirectory(AppContext.BaseDirectory);
        var english = File.ReadAllText(Path.Combine(directory, "en.json"));
        var russian = File.ReadAllText(Path.Combine(directory, "ru.json"));
        var en = JsonSerializer.Deserialize<Dictionary<string, string>>(english)!;
        var ru = JsonSerializer.Deserialize<Dictionary<string, string>>(russian)!;

        Assert.Equal(en.Keys.OrderBy(key => key), ru.Keys.OrderBy(key => key));
        Assert.NotEqual(App.Title, en[App.Title]);
        Assert.NotEqual(en[Tab.Preferences], ru[Tab.Preferences]);
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
        Assert.Equal("Параметры", service[Tab.Preferences]);
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
        Assert.Contains("дн", service.Format(Summary.Days, 5));
    }
}
