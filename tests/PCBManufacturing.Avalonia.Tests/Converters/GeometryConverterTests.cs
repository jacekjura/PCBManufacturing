using System.Globalization;
using Avalonia.Media;
using PCBManufacturing.AvaloniaApp.Converters;

namespace PCBManufacturing.AvaloniaApp.Tests.Converters;

[Collection("Avalonia")]
public sealed class GeometryConverterTests
{
    [Fact]
    public void ParsesMarkupAndEmptyInput()
    {
        var shapes = new GeometryConverter();
        var geometry = Assert.IsAssignableFrom<Geometry>(shapes.Convert("M 0,0 H 12 V 8 Z", typeof(Geometry), null, CultureInfo.InvariantCulture));
        Assert.True(geometry.Bounds.Width > 0 && geometry.Bounds.Height > 0);
        Assert.IsType<StreamGeometry>(shapes.Convert("", typeof(Geometry), null, CultureInfo.InvariantCulture));
    }
}
