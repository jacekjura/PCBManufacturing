using System.Globalization;
using System.Text.Json;
using PCBManufacturing.Core.Json;
using PCBManufacturing.Core.Models;

namespace PCBManufacturing.Core.Catalog;

public interface ICatalogService
{
    ManufacturingCatalog Load();
}

public sealed class JsonCatalogService : ICatalogService
{
    private readonly string _path;
    private ManufacturingCatalog? _cached;

    public JsonCatalogService(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Catalog path is required.", nameof(path));
        _path = path;
    }

    public ManufacturingCatalog Load()
    {
        if (_cached is not null)
            return _cached;

        if (!File.Exists(_path))
            throw new FileNotFoundException("Manufacturing catalog was not found.", _path);

        ManufacturingCatalog? catalog;
        try
        {
            var json = File.ReadAllText(_path);
            catalog = JsonSerializer.Deserialize<ManufacturingCatalog>(json, JsonDefaults.Options);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"Catalog '{_path}' is not valid JSON.", exception);
        }

        if (catalog is null)
            throw new InvalidDataException($"Catalog '{_path}' is empty.");

        Validate(catalog);
        _cached = catalog;
        return catalog;
    }

    public static void Validate(ManufacturingCatalog catalog)
    {
        if (catalog.BasePricePerBoard <= 0)
            throw new InvalidDataException("Base price must be positive.");
        if (string.IsNullOrWhiteSpace(catalog.Currency))
            throw new InvalidDataException("Currency is required.");

        RequireCount(catalog.Materials, 2, "materials");
        RequireCount(catalog.SolderMaskColors, 2, "solder mask colors");
        RequireCount(catalog.Thicknesses, 2, "thicknesses");
        RequireDistinct(catalog.Materials.Select(item => item.Id), "material");
        RequireDistinct(catalog.SolderMaskColors.Select(item => item.Id), "solder mask");
        RequireDistinct(catalog.Thicknesses.Select(item => item.Id), "thickness");
        RequireDistinct(catalog.Parameters.Select(item => item.Id), "parameter");

        foreach (var material in catalog.Materials)
        {
            RequireText(material.Id, "material id");
            RequireText(material.NameKey, "material name");
            RequireModifier(material.PriceModifier, material.Id);
            RequireHex(material.SubstrateHex, material.Id);
            if (material.ProductionDays < 0)
                throw new InvalidDataException($"Material '{material.Id}' has a negative lead time.");
        }

        foreach (var color in catalog.SolderMaskColors)
        {
            RequireText(color.NameKey, "color name");
            RequireHex(color.Hex, color.Id);
        }

        foreach (var thickness in catalog.Thicknesses)
        {
            if (thickness.Millimeters <= 0)
                throw new InvalidDataException($"Thickness '{thickness.Id}' must be positive.");
            RequireModifier(thickness.PriceModifier, thickness.Id);
        }

        foreach (var kind in new[] { ParameterKind.Size, ParameterKind.Quantity, ParameterKind.Layers, ParameterKind.Finish })
        {
            var group = catalog.Parameters.Where(parameter => parameter.Kind == kind).ToList();
            if (group.Count < 2)
                throw new InvalidDataException($"Catalog needs at least two {kind} options.");
            if (group.Count(parameter => parameter.IsDefault) != 1)
                throw new InvalidDataException($"Catalog needs exactly one default {kind} option.");
        }

        foreach (var parameter in catalog.Parameters)
        {
            RequireText(parameter.GroupKey, "parameter group");
            RequireText(parameter.NameKey, "parameter name");
            RequireText(parameter.ValueKey, "parameter value");
            RequireModifier(parameter.PriceModifier, parameter.Id);
            switch (parameter.Kind)
            {
                case ParameterKind.Size when parameter.WidthMm is not > 0 || parameter.HeightMm is not > 0:
                    throw new InvalidDataException($"Size '{parameter.Id}' needs width and height.");
                case ParameterKind.Quantity when parameter.Quantity is not > 0:
                    throw new InvalidDataException($"Quantity '{parameter.Id}' must be positive.");
                case ParameterKind.Layers when parameter.LayerCount is not > 0:
                    throw new InvalidDataException($"Layers '{parameter.Id}' must be positive.");
                case ParameterKind.Finish when string.IsNullOrWhiteSpace(parameter.FinishId):
                    throw new InvalidDataException($"Finish '{parameter.Id}' needs a finish id.");
            }
        }
    }

    private static void RequireCount<T>(ICollection<T> items, int minimum, string name)
    {
        if (items.Count < minimum)
            throw new InvalidDataException($"Catalog needs at least {minimum} {name}.");
    }

    private static void RequireDistinct(IEnumerable<string> ids, string name)
    {
        var list = ids.ToList();
        if (list.Any(string.IsNullOrWhiteSpace) || list.Distinct(StringComparer.OrdinalIgnoreCase).Count() != list.Count)
            throw new InvalidDataException($"Each {name} id must be unique.");
    }

    private static void RequireText(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidDataException($"A {name} is missing.");
    }

    private static void RequireModifier(decimal modifier, string id)
    {
        if (modifier <= 0)
            throw new InvalidDataException($"Price modifier for '{id}' must be positive.");
    }

    private static void RequireHex(string hex, string id)
    {
        if (hex.Length != 7 || hex[0] != '#' || !int.TryParse(hex[1..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
            throw new InvalidDataException($"'{id}' has invalid color '{hex}'.");
    }
}
