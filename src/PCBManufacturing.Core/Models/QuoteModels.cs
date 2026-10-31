namespace PCBManufacturing.Core.Models;

public sealed class QuoteRequest
{
    public required decimal BasePricePerBoard { get; init; }
    public required string Currency { get; init; }
    public required MaterialDefinition Material { get; init; }
    public required BoardThicknessDefinition Thickness { get; init; }
    public required ParameterDefinition Size { get; init; }
    public required ParameterDefinition Layers { get; init; }
    public required ParameterDefinition Finish { get; init; }
    public required ParameterDefinition Quantity { get; init; }
}

public sealed record QuoteImpactLine(string LabelKey, decimal Amount, decimal Modifier, int Days);

public sealed class QuoteResult
{
    public required decimal UnitPrice { get; init; }
    public required decimal Total { get; init; }
    public required int ProductionDays { get; init; }
    public required int Quantity { get; init; }
    public required string Currency { get; init; }
    public required IReadOnlyList<QuoteImpactLine> Lines { get; init; }
}
