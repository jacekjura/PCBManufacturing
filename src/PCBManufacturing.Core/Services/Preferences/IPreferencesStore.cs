using PCBManufacturing.Core.Models.Preferences;

namespace PCBManufacturing.Core.Services.Preferences;

public interface IPreferencesStore
{
    Task<SavedPreferences?> LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(SavedPreferences preferences, CancellationToken cancellationToken = default);
}
