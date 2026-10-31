using System.Windows.Markup;
using PCBManufacturing.Core.Graphics;

namespace PCBManufacturing.Wpf.Markup;

public sealed class IconExtension : MarkupExtension
{
    public IconExtension()
    {
    }

    public IconExtension(string name) => Name = name;

    public string Name { get; set; } = "Logo";

    public override object ProvideValue(IServiceProvider serviceProvider) =>
        IconGeometry.Get(Name).SegoeGlyph;
}
