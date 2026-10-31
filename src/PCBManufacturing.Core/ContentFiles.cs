namespace PCBManufacturing.Core;

public static class ContentFiles
{
    public static string CatalogPath(string contentRoot) =>
        FirstExisting(
            Path.Combine(contentRoot, "catalog.json"),
            Path.Combine(contentRoot, "Data", "catalog.json"));

    public static string LocalizationDirectory(string contentRoot)
    {
        var direct = Path.Combine(contentRoot, "Localization");
        if (File.Exists(Path.Combine(direct, "en.json")))
            return direct;

        var nested = Path.Combine(contentRoot, "Localization", "Localization");
        if (File.Exists(Path.Combine(nested, "en.json")))
            return nested;

        return direct;
    }

    private static string FirstExisting(params string[] candidates) =>
        candidates.FirstOrDefault(File.Exists) ?? candidates[0];
}
