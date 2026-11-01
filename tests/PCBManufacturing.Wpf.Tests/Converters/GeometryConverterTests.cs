using System.Globalization;
using System.Windows.Media;

namespace PCBManufacturing.Wpf.Tests.Converters;

public sealed class GeometryConverterTests
{
    [Fact]
    public void ParsesMarkupAndEmptyInput()
    {
        StaThread.Run(() =>
        {
            var converter = new PCBManufacturing.Wpf.Converters.GeometryConverter();
            var geometry = Assert.IsAssignableFrom<Geometry>(converter.Convert("M 0,0 L 10,0 L 10,4 Z", typeof(Geometry), null, CultureInfo.InvariantCulture));
            Assert.False(geometry.Bounds.IsEmpty);
            Assert.Same(Geometry.Empty, converter.Convert(" ", typeof(Geometry), null, CultureInfo.InvariantCulture));
        });
    }
}
