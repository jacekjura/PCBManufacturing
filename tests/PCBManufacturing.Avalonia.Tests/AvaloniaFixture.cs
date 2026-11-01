using Avalonia;
using Avalonia.Headless;

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
