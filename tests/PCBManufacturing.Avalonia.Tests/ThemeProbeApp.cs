using Avalonia;
using Avalonia.Themes.Fluent;

namespace PCBManufacturing.AvaloniaApp.Tests;

public sealed class ThemeProbeApp : Application
{
    public override void Initialize() => Styles.Add(new FluentTheme());
}
