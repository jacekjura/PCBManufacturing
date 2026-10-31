namespace PCBManufacturing.Core.Graphics;

public readonly record struct IconDefinition(string SegoeGlyph, string FluentSymbol);

public static class IconGeometry
{
    public const string SegoeFontFamily = "Segoe Fluent Icons, Segoe MDL2 Assets";

    public static IReadOnlyList<string> Names { get; } =
    [
        "Logo",
        "Preferences",
        "Quote",
        "Order",
        "Sun",
        "Moon",
        "Check",
        "Alert",
        "Save",
        "Layers"
    ];

    public static IconDefinition Get(string name) => name switch
    {
        "Logo" => new("\uEEA1", "DeveloperBoard"),
        "Preferences" => new("\uE713", "Settings"),
        "Quote" => new("\uE8A5", "Document"),
        "Order" => new("\uE7BF", "Cart"),
        "Sun" => new("\uE706", "WeatherSunny"),
        "Moon" => new("\uE708", "WeatherMoon"),
        "Check" => new("\uE73E", "Checkmark"),
        "Alert" => new("\uE7BA", "Warning"),
        "Save" => new("\uE74E", "Save"),
        "Layers" => new("\uE81E", "Layer"),
        _ => throw new ArgumentOutOfRangeException(nameof(name), name, "Unknown icon.")
    };
}
