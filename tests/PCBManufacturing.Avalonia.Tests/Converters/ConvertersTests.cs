using Avalonia.Media;
using PCBManufacturing.AvaloniaApp.Converters;
using PCBManufacturing.Core.Graphics;

namespace PCBManufacturing.AvaloniaApp.Tests.Converters;

[Collection("Avalonia")]
public sealed class ConvertersTests
{
    [Fact]
    public void HexToBrushConverter_AndGeometryConverter_AcceptCatalogValues()
    {
        var colors = new HexToBrushConverter();
        var brush = Assert.IsType<SolidColorBrush>(colors.Convert("#F4F1EA", typeof(IBrush), null, CultureInfo()));
        Assert.Equal(Color.Parse("#F4F1EA"), brush.Color);
        Assert.Equal(Colors.Transparent, Assert.IsType<SolidColorBrush>(colors.Convert(null, typeof(IBrush), null, CultureInfo())).Color);

        var shapes = new GeometryConverter();
        var geometry = Assert.IsAssignableFrom<Geometry>(shapes.Convert(IconGeometry.Get("Check").Geometry, typeof(Geometry), null, CultureInfo()));
        Assert.True(geometry.Bounds.Width > 0 && geometry.Bounds.Height > 0);
        Assert.IsType<StreamGeometry>(shapes.Convert("", typeof(Geometry), null, CultureInfo()));
    }

    private static System.Globalization.CultureInfo CultureInfo() => System.Globalization.CultureInfo.InvariantCulture;
}
