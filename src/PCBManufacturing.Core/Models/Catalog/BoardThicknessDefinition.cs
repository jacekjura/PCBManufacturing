namespace PCBManufacturing.Core.Models.Catalog;

public sealed class BoardThicknessDefinition
{
    public string Id { get; init; } = "";
    public string NameKey { get; init; } = "";
    public double Millimeters { get; init; }
    public decimal PriceModifier { get; init; } = 1m;
    public int ProductionDaysExtra { get; init; }
}
