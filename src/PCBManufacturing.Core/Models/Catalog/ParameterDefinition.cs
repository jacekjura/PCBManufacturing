namespace PCBManufacturing.Core.Models.Catalog;

public sealed class ParameterDefinition
{
    public string Id { get; init; } = "";
    public string GroupKey { get; init; } = "";
    public ParameterKind Kind { get; init; }
    public string NameKey { get; init; } = "";
    public string ValueKey { get; init; } = "";
    public decimal PriceModifier { get; init; } = 1m;
    public int ProductionDaysExtra { get; init; }
    public double? WidthMm { get; init; }
    public double? HeightMm { get; init; }
    public int? Quantity { get; init; }
    public int? LayerCount { get; init; }
    public string? FinishId { get; init; }
    public bool IsDefault { get; init; }
}
