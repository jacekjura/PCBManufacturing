using PCBManufacturing.Core.Models.Catalog;

namespace PCBManufacturing.Core.Models.Quote;

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
