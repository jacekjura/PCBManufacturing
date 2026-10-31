using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;

namespace PCBManufacturing.AvaloniaApp.Tests;

[Collection("Avalonia")]
public sealed class AppTests
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
    public void FluentTheme_ExposesBrushesUsedByTheShell()
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
    public void DarkPalette_UsesReadableSurfaces()
    {
        var theme = File.ReadAllText(Path.Combine(FindAvaloniaProject(), "App.axaml"));
        Assert.Contains("x:Key=\"Dark\"", theme, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"Brush.App\" Color=\"#1C1C1C\"", theme, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"Brush.Card\" Color=\"#2B2B2B\"", theme, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"Brush.TextPrimary\" Color=\"#FFFFFF\"", theme, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"Brush.Error\" Color=\"#FF6B6B\"", theme, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"Brush.Success\" Color=\"#6CCB5F\"", theme, StringComparison.Ordinal);
        Assert.Contains("x:Key=\"Brush.Preview\" Color=\"#141414\"", theme, StringComparison.Ordinal);

        foreach (var file in Directory.EnumerateFiles(FindAvaloniaProject(), "*.axaml", SearchOption.AllDirectories))
        {
            if (string.Equals(Path.GetFileName(file), "App.axaml", StringComparison.OrdinalIgnoreCase))
                continue;

            var text = File.ReadAllText(file);
            Assert.DoesNotContain("SystemControlBackgroundChromeWhiteBrush", text, StringComparison.Ordinal);
            Assert.DoesNotContain("SystemControlErrorTextForegroundBrush", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Views_DoNotBindRemovedFluentResources()
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
