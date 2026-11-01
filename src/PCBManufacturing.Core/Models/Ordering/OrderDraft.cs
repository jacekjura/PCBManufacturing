namespace PCBManufacturing.Core.Models.Ordering;

public sealed class OrderDraft
{
    public required string ProjectName { get; init; }
    public required string Postcode { get; init; }
    public required string Notes { get; init; }
    public required string MaterialKey { get; init; }
    public required string MaskKey { get; init; }
    public required string MaskHex { get; init; }
    public required string ThicknessKey { get; init; }
    public required string SizeKey { get; init; }
    public required string FinishKey { get; init; }
    public required int Layers { get; init; }
    public required int Quantity { get; init; }
    public required decimal Total { get; init; }
    public required string Currency { get; init; }
    public required int ProductionDays { get; init; }
}
