using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using PCBManufacturing.Core.Models.Theming;

namespace PCBManufacturing.Wpf.Behaviors;

public static class WindowTheme
{
    public static readonly DependencyProperty ThemeProperty = DependencyProperty.RegisterAttached(
        "Theme",
        typeof(AppTheme),
        typeof(WindowTheme),
        new PropertyMetadata(AppTheme.Light, OnThemeChanged));
    public static void SetTheme(DependencyObject element, AppTheme value) => element.SetValue(ThemeProperty, value);
    public static AppTheme GetTheme(DependencyObject element) => (AppTheme)element.GetValue(ThemeProperty);
    private static void OnThemeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not Window window)
            return;
        Apply(window);
        window.SourceInitialized -= OnSourceInitialized;
        window.SourceInitialized += OnSourceInitialized;
    }
    private static void OnSourceInitialized(object? sender, EventArgs args)
    {
        if (sender is Window window)
            Apply(window);
    }
    private static void Apply(Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero)
            return;
        var dark = GetTheme(window) == AppTheme.Dark ? 1 : 0;
        _ = DwmSetWindowAttribute(handle, 20, ref dark, sizeof(int));
    }
    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
}
