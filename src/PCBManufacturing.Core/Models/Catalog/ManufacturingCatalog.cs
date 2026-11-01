namespace PCBManufacturing.Core.Models.Catalog;

public sealed class ManufacturingCatalog
{
    public decimal BasePricePerBoard { get; init; }
    public string Currency { get; init; } = "USD";
    public List<MaterialDefinition> Materials { get; init; } = [];
    public List<SolderMaskColorDefinition> SolderMaskColors { get; init; } = [];
    public List<BoardThicknessDefinition> Thicknesses { get; init; } = [];
    public List<ParameterDefinition> Parameters { get; init; } = [];
}
