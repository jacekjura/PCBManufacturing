using System.Globalization;
using PCBManufacturing.Wpf.Converters;

namespace PCBManufacturing.Wpf.Tests.Converters;

public sealed class BoolToDoubleConverterTests
{
    [Fact]
    public void ReturnsZeroWhenCollapsed()
    {
        var converter = new BoolToDoubleConverter();
        Assert.Equal(0d, converter.Convert(false, typeof(double), "260", CultureInfo.InvariantCulture));
        Assert.Equal(260d, converter.Convert(true, typeof(double), "260", CultureInfo.InvariantCulture));
    }
}
