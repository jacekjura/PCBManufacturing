namespace PCBManufacturing.Core.Models.Catalog;

public sealed class MaterialDefinition
{
    public string Id { get; init; } = "";
    public string NameKey { get; init; } = "";
    public string DescriptionKey { get; init; } = "";
    public decimal PriceModifier { get; init; } = 1m;
    public int ProductionDays { get; init; }
    public string SubstrateHex { get; init; } = "#C4A574";
}
