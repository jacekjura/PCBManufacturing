using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PCBManufacturing.Core.Graphics;

namespace PCBManufacturing.AvaloniaApp.Markup;

public sealed class IconExtension : MarkupExtension
{
    public IconExtension()
    {
    }

    public IconExtension(string name) => Name = name;

    public string Name { get; set; } = "Logo";

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        Geometry.Parse(IconGeometry.Get(Name).Geometry);
}
