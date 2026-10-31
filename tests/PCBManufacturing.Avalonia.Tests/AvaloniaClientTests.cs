using Avalonia;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using PCBManufacturing.AvaloniaApp.Converters;
using PCBManufacturing.Core.Graphics;

namespace PCBManufacturing.AvaloniaApp.Tests;

[Collection("Avalonia")]
public sealed class AvaloniaClientTests
{
    private static readonly string[] FluentBrushes =
    [
        "SystemControlBackgroundAccentBrush",
        "SystemControlBackgroundChromeMediumBrush",
        "SystemControlBackgroundAltHighBrush",
        "SystemControlForegroundBaseHighBrush",
        "SystemControlForegroundBaseMediumLowBrush",
        "SystemControlForegroundChromeWhiteBrush",
        "SystemControlForegroundAccentBrush",
        "SystemControlErrorTextForegroundBrush",
        "SystemControlHighlightAccentBrush",
        "SystemControlBackgroundListLowBrush"
    ];

    [Fact]
    public void Fluent_theme_exposes_brushes_used_by_the_shell()
    {
        var app = Application.Current ?? throw new InvalidOperationException("Avalonia was not initialized.");
        foreach (var key in FluentBrushes)
        {
            Assert.True(app.TryGetResource(key, ThemeVariant.Light, out var light), key);
            Assert.IsAssignableFrom<IBrush>(light);
            Assert.True(app.TryGetResource(key, ThemeVariant.Dark, out var dark), key);
            Assert.IsAssignableFrom<IBrush>(dark);
        }
    }

    [Fact]
    public void Vector_icons_parse_as_avalonia_geometry()
    {
        foreach (var name in IconGeometry.Names)
        {
            var icon = IconGeometry.Get(name);
            var geometry = Geometry.Parse(icon.Geometry);
            Assert.True(geometry.Bounds.Width > 0 && geometry.Bounds.Height > 0, name);
        }
    }

    [Fact]
    public void Hex_and_geometry_converters_accept_catalog_values()
    {
        var colors = new HexToBrushConverter();
        var brush = Assert.IsType<SolidColorBrush>(colors.Convert("#F4F1EA", typeof(IBrush), null, CultureInfo()));
        Assert.Equal(Color.Parse("#F4F1EA"), brush.Color);
        Assert.Equal(Colors.Transparent, Assert.IsType<SolidColorBrush>(colors.Convert(null, typeof(IBrush), null, CultureInfo())).Color);

        var shapes = new GeometryConverter();
        var geometry = Assert.IsAssignableFrom<Geometry>(shapes.Convert(IconGeometry.Get("Check").Geometry, typeof(Geometry), null, CultureInfo()));
        Assert.True(geometry.Bounds.Width > 0 && geometry.Bounds.Height > 0);
        Assert.IsType<StreamGeometry>(shapes.Convert("", typeof(Geometry), null, CultureInfo()));
    }

    [Fact]
    public void Views_do_not_bind_removed_fluent_resources()
    {
        var root = FindAvaloniaProject();
        var forbidden = new[]
        {
            "SystemAccentColor}",
            "SystemAccentColorLight1",
            "SystemFillColorSuccessBrush",
            "SystemControlErrorTextBackgroundBrush"
        };

        var files = Directory.EnumerateFiles(root, "*.axaml", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var text = File.ReadAllText(file);
            foreach (var key in forbidden)
                Assert.DoesNotContain(key, text, StringComparison.Ordinal);
        }
    }

    private static System.Globalization.CultureInfo CultureInfo() => System.Globalization.CultureInfo.InvariantCulture;

    private static string FindAvaloniaProject()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "src", "PCBManufacturing.Avalonia");
            if (Directory.Exists(candidate))
                return candidate;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the Avalonia project from the test output directory.");
    }
}

public sealed class AvaloniaFixture
{
    public AvaloniaFixture()
    {
        AppBuilder.Configure<ThemeProbeApp>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();
    }
}

public sealed class ThemeProbeApp : Application
{
    public override void Initialize() => Styles.Add(new FluentTheme());
}

[CollectionDefinition("Avalonia")]
public sealed class AvaloniaCollection : ICollectionFixture<AvaloniaFixture>;
