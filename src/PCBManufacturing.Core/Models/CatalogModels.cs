namespace PCBManufacturing.Core.Models;

public enum AppTheme
{
    Light,
    Dark
}

public enum ParameterKind
{
    Size,
    Quantity,
    Layers,
    Finish
}

public enum ShapeRole
{
    Substrate,
    Mask,
    Copper,
    Core,
    Pad,
    Trace,
    Hole,
    Silk
}

public sealed class MaterialDefinition
{
    public string Id { get; init; } = "";
    public string NameKey { get; init; } = "";
    public string DescriptionKey { get; init; } = "";
    public decimal PriceModifier { get; init; } = 1m;
    public int ProductionDays { get; init; }
    public string SubstrateHex { get; init; } = "#C4A574";
}

public sealed class SolderMaskColorDefinition
{
    public string Id { get; init; } = "";
    public string NameKey { get; init; } = "";
    public string Hex { get; init; } = "#1B7A3A";
}

public sealed class BoardThicknessDefinition
{
    public string Id { get; init; } = "";
    public string NameKey { get; init; } = "";
    public double Millimeters { get; init; }
    public decimal PriceModifier { get; init; } = 1m;
    public int ProductionDaysExtra { get; init; }
}

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

public sealed class ManufacturingCatalog
{
    public decimal BasePricePerBoard { get; init; }
    public string Currency { get; init; } = "USD";
    public List<MaterialDefinition> Materials { get; init; } = [];
    public List<SolderMaskColorDefinition> SolderMaskColors { get; init; } = [];
    public List<BoardThicknessDefinition> Thicknesses { get; init; } = [];
    public List<ParameterDefinition> Parameters { get; init; } = [];
}
