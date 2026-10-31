using FluentIcons.Avalonia;
using FluentIcons.Common;
using PCBManufacturing.AvaloniaApp.Markup;
using PCBManufacturing.Core.Graphics;

namespace PCBManufacturing.AvaloniaApp.Tests.Graphics;

[Collection("Avalonia")]
public sealed class IconGeometryTests
{
    [Fact]
    public void Icons_ResolveToFluentSymbols()
    {
        foreach (var name in IconGeometry.Names)
        {
            var symbol = Assert.IsType<Symbol>(new IconExtension(name).ProvideValue(null!));
            Assert.Equal(IconGeometry.Get(name).FluentSymbol, symbol.ToString());
            var icon = new SymbolIcon { Symbol = symbol, FontSize = 16 };
            Assert.Equal(symbol, icon.Symbol);
        }
    }
}
