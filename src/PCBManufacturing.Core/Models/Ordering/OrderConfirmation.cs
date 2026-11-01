namespace PCBManufacturing.Core.Models.Ordering;

public sealed class OrderConfirmation
{
    public string Number { get; init; } = "";
    public DateTimeOffset CreatedAt { get; init; }
    public string ProjectName { get; init; } = "";
    public string Postcode { get; init; } = "";
    public string Notes { get; init; } = "";
    public string MaterialKey { get; init; } = "";
    public string MaskKey { get; init; } = "";
    public string MaskHex { get; init; } = "#1B7A3A";
    public string ThicknessKey { get; init; } = "";
    public string SizeKey { get; init; } = "";
    public string FinishKey { get; init; } = "";
    public int Layers { get; init; }
    public int Quantity { get; init; }
    public decimal Total { get; init; }
    public string Currency { get; init; } = "USD";
    public int ProductionDays { get; init; }
}
