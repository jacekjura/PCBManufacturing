using Avalonia;
using Avalonia.Styling;
using PCBManufacturing.Core.Models;
using PCBManufacturing.Core.Theming;

namespace PCBManufacturing.AvaloniaApp.Services;

public sealed class AvaloniaThemeApplier : IThemeApplier
{
    public void Apply(AppTheme theme)
    {
        if (Application.Current is null)
            return;

        Application.Current.RequestedThemeVariant = theme == AppTheme.Dark
            ? ThemeVariant.Dark
            : ThemeVariant.Light;
    }
}
