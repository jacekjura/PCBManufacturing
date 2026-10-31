using System.IO;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using PCBManufacturing.Core.Graphics;
using PCBManufacturing.Wpf.Converters;

namespace PCBManufacturing.Wpf.Tests;

public sealed class WpfClientTests
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

    [Fact]
    public void Vector_icons_parse_as_wpf_geometry()
    {
        RunOnSta(() =>
        {
            foreach (var name in IconGeometry.Names)
            {
                var icon = IconGeometry.Get(name);
                var geometry = Geometry.Parse(icon.Geometry);
                Assert.False(geometry.Bounds.IsEmpty, name);
            }
        });
    }

    [Fact]
    public void Hex_converter_reads_rgb_colors()
    {
        RunOnSta(() =>
        {
            var converter = new HexToBrushConverter();
            var brush = Assert.IsType<SolidColorBrush>(converter.Convert("#1B7A3A", typeof(Brush), null, CultureInfo()));
            Assert.Equal(Color.FromRgb(0x1B, 0x7A, 0x3A), brush.Color);
            Assert.True(brush.IsFrozen);

            var missing = Assert.IsType<SolidColorBrush>(converter.Convert("nope", typeof(Brush), null, CultureInfo()));
            Assert.Equal(Colors.Transparent, missing.Color);
        });
    }

    [Fact]
    public void Geometry_converter_parses_markup_and_empty_input()
    {
        RunOnSta(() =>
        {
            var converter = new Converters.GeometryConverter();
            var geometry = Assert.IsAssignableFrom<Geometry>(converter.Convert("M 0,0 L 10,0 L 10,4 Z", typeof(Geometry), null, CultureInfo()));
            Assert.False(geometry.Bounds.IsEmpty);
            Assert.Same(Geometry.Empty, converter.Convert(" ", typeof(Geometry), null, CultureInfo()));
        });
    }

    [Fact]
    public void Visibility_converter_honors_invert()
    {
        var converter = new BoolToVisibilityConverter();
        Assert.Equal(Visibility.Visible, converter.Convert(true, typeof(Visibility), null, CultureInfo()));
        Assert.Equal(Visibility.Collapsed, converter.Convert(false, typeof(Visibility), null, CultureInfo()));
        Assert.Equal(Visibility.Collapsed, converter.Convert(true, typeof(Visibility), "Invert", CultureInfo()));
        Assert.Equal(true, converter.ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo()));
    }

    [Theory]
    [InlineData("Light.xaml")]
    [InlineData("Dark.xaml")]
    public void Theme_dictionary_defines_the_windows11_palette(string fileName)
    {
        RunOnSta(() =>
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Themes", fileName);
            using var stream = File.OpenRead(path);
            var dictionary = Assert.IsType<ResourceDictionary>(XamlReader.Load(stream));
            foreach (var key in ThemeBrushes)
                Assert.IsType<SolidColorBrush>(dictionary[key]);
        });
    }

    private static System.Globalization.CultureInfo CultureInfo() => System.Globalization.CultureInfo.InvariantCulture;

    private static void RunOnSta(Action action)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure is not null)
            ExceptionDispatchInfo.Capture(failure).Throw();
    }
}
