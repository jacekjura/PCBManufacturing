using System.Text.Json;
using PCBManufacturing.Core.Json;
using PCBManufacturing.Core.Models;

namespace PCBManufacturing.Core.Preferences;

public interface IPreferencesStore
{
    Task<SavedPreferences?> LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(SavedPreferences preferences, CancellationToken cancellationToken = default);
}

public sealed class JsonPreferencesStore : IPreferencesStore
{
    private readonly string _path;

    public JsonPreferencesStore(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Preferences path is required.", nameof(path));
        _path = path;
    }

    public async Task<SavedPreferences?> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_path))
            return null;

        try
        {
            await using var stream = File.OpenRead(_path);
            return await JsonSerializer.DeserializeAsync<SavedPreferences>(stream, JsonDefaults.Options, cancellationToken).ConfigureAwait(false);
        }
        catch (JsonException)
        {
            var backup = _path + ".invalid";
            File.Copy(_path, backup, overwrite: true);
            return null;
        }
    }

    public async Task SaveAsync(SavedPreferences preferences, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(preferences);
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var temporary = _path + ".tmp";
        await using (var stream = File.Create(temporary))
        {
            await JsonSerializer.SerializeAsync(stream, preferences, JsonDefaults.Options, cancellationToken).ConfigureAwait(false);
        }

        File.Move(temporary, _path, overwrite: true);
    }
}
