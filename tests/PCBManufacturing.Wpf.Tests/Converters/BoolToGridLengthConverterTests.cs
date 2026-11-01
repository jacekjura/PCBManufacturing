using System.Globalization;
using System.Windows;
using PCBManufacturing.Wpf.Converters;

namespace PCBManufacturing.Wpf.Tests.Converters;

public sealed class BoolToGridLengthConverterTests
{
    [Fact]
    public void UsesStarWhenExpanded()
    {
        var converter = new BoolToGridLengthConverter();
        var collapsed = Assert.IsType<GridLength>(converter.Convert(false, typeof(GridLength), "3", CultureInfo.InvariantCulture));
        Assert.True(collapsed.IsAuto);

        var expanded = Assert.IsType<GridLength>(converter.Convert(true, typeof(GridLength), "3", CultureInfo.InvariantCulture));
        Assert.True(expanded.IsStar);
        Assert.Equal(3, expanded.Value);
    }
}
