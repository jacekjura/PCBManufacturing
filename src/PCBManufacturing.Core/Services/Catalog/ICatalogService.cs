using PCBManufacturing.Core.Models.Catalog;

namespace PCBManufacturing.Core.Services.Catalog;

public interface ICatalogService
{
    ManufacturingCatalog Load();
}
