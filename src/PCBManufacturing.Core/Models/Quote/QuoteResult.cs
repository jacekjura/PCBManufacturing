namespace PCBManufacturing.Core.Models.Quote;

public sealed class QuoteResult
{
    public required decimal UnitPrice { get; init; }
    public required decimal Total { get; init; }
    public required int ProductionDays { get; init; }
    public required int Quantity { get; init; }
    public required string Currency { get; init; }
    public required IReadOnlyList<QuoteImpactLine> Lines { get; init; }
}
