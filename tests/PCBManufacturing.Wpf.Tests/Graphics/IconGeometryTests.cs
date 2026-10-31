using PCBManufacturing.Core.Graphics;
using PCBManufacturing.Wpf.Markup;

namespace PCBManufacturing.Wpf.Tests.Graphics;

public sealed class IconGeometryTests
{
    [Fact]
    public void Icons_ResolveToSegoeGlyphs()
    {
        StaThread.Run(() =>
        {
            foreach (var name in IconGeometry.Names)
            {
                var glyph = Assert.IsType<string>(new IconExtension(name).ProvideValue(null!));
                Assert.Equal(IconGeometry.Get(name).SegoeGlyph, glyph);
            }
        });
    }
}
