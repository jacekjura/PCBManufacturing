using PCBManufacturing.Core.Graphics;
using PCBManufacturing.Core.Models;

namespace PCBManufacturing.Core.Tests.Graphics;

public class PcbArtworkFactoryTests
{
    private readonly PcbArtworkFactory _factory = new();

    [Fact]
    public void Board_UsesMaskColorAndDrawsPadsTracesAndHoles()
    {
        var shapes = _factory.CreateBoard(Request(layers: 2));

        Assert.Contains(shapes, shape => shape.Role == ShapeRole.Mask && shape.FillHex == "#1A4F8B");
        Assert.Contains(shapes, shape => shape.Role == ShapeRole.Pad && shape.FillHex == PcbArtworkFactory.FinishColor("enig"));
        Assert.Contains(shapes, shape => shape.Role == ShapeRole.Trace);
        Assert.True(shapes.Count(shape => shape.Role == ShapeRole.Hole) >= 8);
        Assert.All(shapes, shape => Assert.False(string.IsNullOrWhiteSpace(shape.Geometry)));
    }

    [Fact]
    public void LayerStack_GrowsWithTheLayerCount()
    {
        var two = _factory.CreateLayerStack(Request(layers: 2)).Count(shape => shape.Role is ShapeRole.Mask or ShapeRole.Copper or ShapeRole.Core);
        var six = _factory.CreateLayerStack(Request(layers: 6)).Count(shape => shape.Role is ShapeRole.Mask or ShapeRole.Copper or ShapeRole.Core);

        Assert.Equal(5, two);
        Assert.Equal(13, six);
    }

    private static PcbArtworkRequest Request(int layers) => new()
    {
        WidthMm = 100,
        HeightMm = 80,
        LayerCount = layers,
        ThicknessMm = 1.6,
        MaskHex = "#1A4F8B",
        SubstrateHex = "#C4A574",
        FinishId = "enig"
    };
}
