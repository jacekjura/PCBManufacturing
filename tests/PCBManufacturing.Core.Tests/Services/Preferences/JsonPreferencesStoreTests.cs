using PCBManufacturing.Core.Models;
using PCBManufacturing.Core.Preferences;

namespace PCBManufacturing.Core.Tests.Preferences;

public class JsonPreferencesStoreTests
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
