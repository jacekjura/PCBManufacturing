using Avalonia.Controls;
using Avalonia.Platform;

namespace PCBManufacturing.AvaloniaApp;

public static class AppIcons
{
    public static WindowIcon Window { get; } = new(AssetLoader.Open(
        new Uri("avares://PCBManufacturing.Avalonia/Assets/AppIcon.ico")));
}
