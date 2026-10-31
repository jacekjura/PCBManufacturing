using System.Globalization;

namespace PCBManufacturing.Core.Formatting;

public static class Money
{
    public static string Format(decimal amount, string currency, CultureInfo culture)
    {
        var format = (NumberFormatInfo)culture.NumberFormat.Clone();
        format.CurrencySymbol = currency switch
        {
            "USD" => "$",
            "EUR" => "€",
            "GBP" => "£",
            _ => currency + " "
        };
        format.CurrencyPositivePattern = 0;
        format.CurrencyNegativePattern = 1;
        return amount.ToString("C2", format);
    }
}
