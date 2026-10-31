using Avalonia;
using Avalonia.Headless;
using Avalonia.Themes.Fluent;

namespace PCBManufacturing.AvaloniaApp.Tests;

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
