using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace PCBManufacturing.Wpf.Tests.Themes;

public sealed class ThemeTests
{
    private static readonly string[] ThemeBrushes =
    [
        "Brush.App",
        "Brush.Card",
        "Brush.CardAlt",
        "Brush.Stroke",
        "Brush.TextPrimary",
        "Brush.TextSecondary",
        "Brush.Accent",
        "Brush.AccentHover",
        "Brush.AccentPressed",
        "Brush.AccentSubtle",
        "Brush.OnAccent",
        "Brush.Input",
        "Brush.Hover",
        "Brush.Error",
        "Brush.ErrorSubtle",
        "Brush.Success",
        "Brush.SuccessSubtle",
        "Brush.Preview"
    ];

    [Theory]
    [InlineData("Light.xaml")]
    [InlineData("Dark.xaml")]
    public void Dictionary_DefinesTheWindows11Palette(string fileName)
    {
        StaThread.Run(() =>
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Themes", fileName);
            using var stream = File.OpenRead(path);
            var dictionary = Assert.IsType<ResourceDictionary>(XamlReader.Load(stream));
            foreach (var key in ThemeBrushes)
                Assert.IsType<SolidColorBrush>(dictionary[key]);
        });
    }
}
