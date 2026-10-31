using System.Globalization;
using System.Windows;
using System.Windows.Media;
using PCBManufacturing.Wpf.Converters;

namespace PCBManufacturing.Wpf.Tests.Converters;

public sealed class ConvertersTests
{
    [Fact]
    public void HexToBrushConverter_ReadsRgbColors()
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

    [Fact]
    public void GeometryConverter_ParsesMarkupAndEmptyInput()
    {
        StaThread.Run(() =>
        {
            var converter = new PCBManufacturing.Wpf.Converters.GeometryConverter();
            var geometry = Assert.IsAssignableFrom<Geometry>(converter.Convert("M 0,0 L 10,0 L 10,4 Z", typeof(Geometry), null, CultureInfo.InvariantCulture));
            Assert.False(geometry.Bounds.IsEmpty);
            Assert.Same(Geometry.Empty, converter.Convert(" ", typeof(Geometry), null, CultureInfo.InvariantCulture));
        });
    }

    [Fact]
    public void BoolToVisibilityConverter_HonorsInvert()
    {
        var converter = new BoolToVisibilityConverter();
        Assert.Equal(Visibility.Visible, converter.Convert(true, typeof(Visibility), null, CultureInfo.InvariantCulture));
        Assert.Equal(Visibility.Collapsed, converter.Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture));
        Assert.Equal(Visibility.Collapsed, converter.Convert(true, typeof(Visibility), "Invert", CultureInfo.InvariantCulture));
        Assert.Equal(true, converter.ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void BoolToGridLengthConverter_UsesStarWhenExpanded()
    {
        var converter = new BoolToGridLengthConverter();
        var collapsed = Assert.IsType<GridLength>(converter.Convert(false, typeof(GridLength), "3", CultureInfo.InvariantCulture));
        Assert.True(collapsed.IsAuto);

        var expanded = Assert.IsType<GridLength>(converter.Convert(true, typeof(GridLength), "3", CultureInfo.InvariantCulture));
        Assert.True(expanded.IsStar);
        Assert.Equal(3, expanded.Value);
    }

    [Fact]
    public void BoolToDoubleConverter_ReturnsZeroWhenCollapsed()
    {
        var converter = new BoolToDoubleConverter();
        Assert.Equal(0d, converter.Convert(false, typeof(double), "260", CultureInfo.InvariantCulture));
        Assert.Equal(260d, converter.Convert(true, typeof(double), "260", CultureInfo.InvariantCulture));
    }
}
