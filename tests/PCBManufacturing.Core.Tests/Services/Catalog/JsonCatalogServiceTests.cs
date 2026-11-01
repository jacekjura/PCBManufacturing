using PCBManufacturing.Core;
using PCBManufacturing.Core.Services.Catalog;
using PCBManufacturing.Core.Models.Catalog;

namespace PCBManufacturing.Core.Tests.Services.Catalog;

public class JsonCatalogServiceTests
{
    [Fact]
    public void ShippedCatalog_HasMaterialsColorsThicknessesAndOneDefaultPerGroup()
    {
        var catalog = new JsonCatalogService(ContentFiles.CatalogPath(AppContext.BaseDirectory)).Load();

        Assert.True(catalog.BasePricePerBoard > 0);
        Assert.True(catalog.Materials.Count >= 3);
        Assert.True(catalog.SolderMaskColors.Count >= 3);
        Assert.True(catalog.Thicknesses.Count >= 3);
        Assert.Equal(1, catalog.Parameters.Count(parameter => parameter.Kind == ParameterKind.Size && parameter.IsDefault));
        Assert.Equal(1, catalog.Parameters.Count(parameter => parameter.Kind == ParameterKind.Layers && parameter.IsDefault));
        Assert.Equal(1, catalog.Parameters.Count(parameter => parameter.Kind == ParameterKind.Finish && parameter.IsDefault));
        Assert.Equal(1, catalog.Parameters.Count(parameter => parameter.Kind == ParameterKind.Quantity && parameter.IsDefault));
        Assert.Contains(catalog.Materials, material => material.PriceModifier != 1m && material.ProductionDays > 0);
    }

    [Fact]
    public void InvalidCatalog_IsRejected()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        File.WriteAllText(path, """{"basePricePerBoard": 0, "currency": "USD"}""");

        var exception = Assert.Throws<InvalidDataException>(() => new JsonCatalogService(path).Load());
        Assert.Contains("Base price", exception.Message);
    }
}
