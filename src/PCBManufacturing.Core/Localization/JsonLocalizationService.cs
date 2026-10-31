using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using PCBManufacturing.Core.Json;

namespace PCBManufacturing.Core.Localization;

public interface ILocalizationService : INotifyPropertyChanged
{
    string Culture { get; }
    CultureInfo CultureInfo { get; }
    string this[string key] { get; }
    void SetCulture(string culture);
    string Format(string key, params object[] args);
}

public sealed class JsonLocalizationService : ILocalizationService
{
    private readonly string _directory;
    private readonly Dictionary<string, string> _english;
    private Dictionary<string, string> _active;
    private string _culture;

    public JsonLocalizationService(string localizationDirectory, string culture = "en")
    {
        if (string.IsNullOrWhiteSpace(localizationDirectory))
            throw new ArgumentException("Localization directory is required.", nameof(localizationDirectory));

        _directory = localizationDirectory;
        _english = Read(Path.Combine(_directory, "en.json"));
        _active = _english;
        _culture = "en";
        CultureInfo = CultureInfo.GetCultureInfo("en-US");
        if (!culture.Equals("en", StringComparison.OrdinalIgnoreCase))
            SetCulture(culture);
    }

    public string Culture => _culture;

    public CultureInfo CultureInfo { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string this[string key]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;
            if (_active.TryGetValue(key, out var value))
                return value;
            if (_english.TryGetValue(key, out var fallback))
                return fallback;
            return key;
        }
    }

    public void SetCulture(string culture)
    {
        var normalized = culture.StartsWith("ru", StringComparison.OrdinalIgnoreCase) ? "ru" : "en";
        if (normalized == _culture)
            return;

        _active = normalized == "en" ? _english : Read(Path.Combine(_directory, "ru.json"));
        _culture = normalized;
        CultureInfo = CultureInfo.GetCultureInfo(normalized == "ru" ? "ru-RU" : "en-US");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item"));
    }

    public string Format(string key, params object[] args) =>
        string.Format(CultureInfo, this[key], args);

    private static Dictionary<string, string> Read(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Localization file was not found.", path);

        try
        {
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path), JsonDefaults.Options);
            if (dictionary is null || dictionary.Count == 0)
                throw new InvalidDataException($"Localization file '{path}' is empty.");
            return dictionary;
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"Localization file '{path}' is not valid JSON.", exception);
        }
    }
}
