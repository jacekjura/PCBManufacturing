using PCBManufacturing.Core.Models.Artwork;

namespace PCBManufacturing.Core.Graphics;

public interface IPcbArtworkFactory
{
    IReadOnlyList<VectorShape> CreateBoard(PcbArtworkRequest request);
    IReadOnlyList<VectorShape> CreateLayerStack(PcbArtworkRequest request);
}
