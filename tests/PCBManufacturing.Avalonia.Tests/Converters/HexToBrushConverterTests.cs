using System.Globalization;
using Avalonia.Media;
using PCBManufacturing.AvaloniaApp.Converters;

namespace PCBManufacturing.AvaloniaApp.Tests.Converters;

[Collection("Avalonia")]
public sealed class HexToBrushConverterTests
{
    [Fact]
    public void AcceptsCatalogColors()
    {
        var colors = new HexToBrushConverter();
        var brush = Assert.IsType<SolidColorBrush>(colors.Convert("#F4F1EA", typeof(IBrush), null, CultureInfo.InvariantCulture));
        Assert.Equal(Color.Parse("#F4F1EA"), brush.Color);
        Assert.Equal(Colors.Transparent, Assert.IsType<SolidColorBrush>(colors.Convert(null, typeof(IBrush), null, CultureInfo.InvariantCulture)).Color);
    }
}
