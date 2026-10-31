using System.Globalization;
using PCBManufacturing.Core.Formatting;

namespace PCBManufacturing.Core.Tests.Formatting;

public class MoneyTests
{
    [Fact]
    public void Format_UsesTheRequestedCurrencySymbol()
    {
        var formatted = Money.Format(12.5m, "USD", CultureInfo.GetCultureInfo("en-US"));
        Assert.Equal("$12.50", formatted);
    }
}
