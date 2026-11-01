using PCBManufacturing.Core.Models.Catalog;
using PCBManufacturing.Core.Models.Theming;

namespace PCBManufacturing.Core.Models.Preferences;

public sealed record SavedPreferences
{
    public string ProjectName { get; init; } = "AC-9001";
    public string Postcode { get; init; } = "";
    public string Notes { get; init; } = "";
    public string? MaterialId { get; init; }
    public string? SolderMaskColorId { get; init; }
    public string? ThicknessId { get; init; }
    public string? QuantityId { get; init; }
    public string? SizeId { get; init; }
    public string? LayersId { get; init; }
    public string? FinishId { get; init; }
    public AppTheme Theme { get; init; } = AppTheme.Light;
    public string Culture { get; init; } = "en";

    public static SavedPreferences CreateDefault(ManufacturingCatalog catalog)
    {
        string? DefaultId(ParameterKind kind) =>
            catalog.Parameters.FirstOrDefault(parameter => parameter.Kind == kind && parameter.IsDefault)?.Id
            ?? catalog.Parameters.FirstOrDefault(parameter => parameter.Kind == kind)?.Id;

        return new SavedPreferences
        {
            MaterialId = catalog.Materials.FirstOrDefault()?.Id,
            SolderMaskColorId = catalog.SolderMaskColors.FirstOrDefault()?.Id,
            ThicknessId = catalog.Thicknesses.FirstOrDefault(item => item.Millimeters == 1.6)?.Id
                ?? catalog.Thicknesses.FirstOrDefault()?.Id,
            QuantityId = DefaultId(ParameterKind.Quantity),
            SizeId = DefaultId(ParameterKind.Size),
            LayersId = DefaultId(ParameterKind.Layers),
            FinishId = DefaultId(ParameterKind.Finish)
        };
    }
}
