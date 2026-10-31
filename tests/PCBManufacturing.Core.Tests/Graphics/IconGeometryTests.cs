using PCBManufacturing.Core.Graphics;

namespace PCBManufacturing.Core.Tests.Graphics;

public class IconGeometryTests
{
    [Fact]
    public void Icons_UseSegoeFluentGlyphsAndFluentSymbols()
    {
        var glyphs = new HashSet<string>();
        var symbols = new HashSet<string>();
        foreach (var name in IconGeometry.Names)
        {
            var icon = IconGeometry.Get(name);
            Assert.Single(icon.SegoeGlyph);
            Assert.InRange(icon.SegoeGlyph[0], (char)0xE000, (char)0xF8FF);
            Assert.Matches("^[A-Z][A-Za-z0-9]+$", icon.FluentSymbol);
            Assert.True(glyphs.Add(icon.SegoeGlyph), name);
            Assert.True(symbols.Add(icon.FluentSymbol), name);
        }

        Assert.Equal("Segoe Fluent Icons, Segoe MDL2 Assets", IconGeometry.SegoeFontFamily);
        Assert.Equal("\uE713", IconGeometry.Get("Preferences").SegoeGlyph);
        Assert.Equal("Settings", IconGeometry.Get("Preferences").FluentSymbol);
    }

    [Fact]
    public void UnknownIcon_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => IconGeometry.Get("Missing"));
}
