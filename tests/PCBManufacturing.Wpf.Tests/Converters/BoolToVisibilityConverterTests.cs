using System.Globalization;
using System.Windows;
using PCBManufacturing.Wpf.Converters;

namespace PCBManufacturing.Wpf.Tests.Converters;

public sealed class BoolToVisibilityConverterTests
{
    [Fact]
    public void HonorsInvert()
    {
        var converter = new BoolToVisibilityConverter();
        Assert.Equal(Visibility.Visible, converter.Convert(true, typeof(Visibility), null, CultureInfo.InvariantCulture));
        Assert.Equal(Visibility.Collapsed, converter.Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture));
        Assert.Equal(Visibility.Collapsed, converter.Convert(true, typeof(Visibility), "Invert", CultureInfo.InvariantCulture));
        Assert.Equal(true, converter.ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.InvariantCulture));
    }
}
