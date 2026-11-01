using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace PCBManufacturing.AvaloniaApp.Converters;

public sealed class GeometryConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string markup && !string.IsNullOrWhiteSpace(markup))
            return Geometry.Parse(markup);
        return new StreamGeometry();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
