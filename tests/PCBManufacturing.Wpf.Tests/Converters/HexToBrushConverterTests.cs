using System.Globalization;
using System.Windows.Media;
using PCBManufacturing.Wpf.Converters;

namespace PCBManufacturing.Wpf.Tests.Converters;

public sealed class HexToBrushConverterTests
{
    [Fact]
    public void ReadsRgbColors()
    {
        StaThread.Run(() =>
        {
            var converter = new HexToBrushConverter();
            var brush = Assert.IsType<SolidColorBrush>(converter.Convert("#1B7A3A", typeof(Brush), null, CultureInfo.InvariantCulture));
            Assert.Equal(Color.FromRgb(0x1B, 0x7A, 0x3A), brush.Color);
            Assert.True(brush.IsFrozen);

            var missing = Assert.IsType<SolidColorBrush>(converter.Convert("nope", typeof(Brush), null, CultureInfo.InvariantCulture));
            Assert.Equal(Colors.Transparent, missing.Color);
        });
    }
}
