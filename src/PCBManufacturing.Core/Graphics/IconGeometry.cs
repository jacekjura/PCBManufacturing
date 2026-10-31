namespace PCBManufacturing.Core.Graphics;

public readonly record struct IconDefinition(string Geometry, bool Filled);

public static class IconGeometry
{
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
        "Logo" => new("M 3,7 H 21 A 2,2 0 0 1 23,9 V 17 A 2,2 0 0 1 21,19 H 3 A 2,2 0 0 1 1,17 V 9 A 2,2 0 0 1 3,7 Z M 6,13 H 9 L 11,10 L 13,16 L 15,13 H 18", false),
        "Preferences" => new("M 4,7 H 10 M 14,7 H 20 M 12,5 V 9 M 4,13 H 6 M 10,13 H 20 M 8,11 V 15 M 4,19 H 14 M 18,19 H 20 M 16,17 V 21", false),
        "Quote" => new("M 5,4 H 19 M 5,9 H 19 M 5,14 H 13 M 5,19 H 10", false),
        "Order" => new("M 4,6 H 20 L 18,16 H 7 Z M 4,6 L 3,3 H 1 M 9,20 A 1.2,1.2 0 1 0 9.01,20 M 17,20 A 1.2,1.2 0 1 0 17.01,20", false),
        "Sun" => new("M 12,8 A 4,4 0 1 0 12.01,8 M 12,2 V 5 M 12,19 V 22 M 2,12 H 5 M 19,12 H 22 M 4.9,4.9 L 7,7 M 17,17 L 19.1,19.1 M 19.1,4.9 L 17,7 M 7,17 L 4.9,19.1", false),
        "Moon" => new("M 16,3 A 8,8 0 1 0 21,14 A 6.5,6.5 0 0 1 16,3 Z", true),
        "Check" => new("M 4,12 L 9,17 L 20,6", false),
        "Alert" => new("M 12,3 L 22,20 H 2 Z M 12,9 V 14", false),
        "Save" => new("M 5,3 H 16 L 21,8 V 21 H 5 Z M 7,3 V 9 H 15 V 3 M 8,21 V 14 H 16 V 21", false),
        "Layers" => new("M 12,3 L 22,8 L 12,13 L 2,8 Z M 2,12 L 12,17 L 22,12 M 2,16 L 12,21 L 22,16", false),
        _ => throw new ArgumentOutOfRangeException(nameof(name), name, "Unknown icon.")
    };
}
