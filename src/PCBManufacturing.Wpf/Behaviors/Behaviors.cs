using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using PCBManufacturing.Core.Models;

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

public static class FadeIn
{
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled",
        typeof(bool),
        typeof(FadeIn),
        new PropertyMetadata(false, OnChanged));

    public static void SetIsEnabled(DependencyObject element, bool value) => element.SetValue(IsEnabledProperty, value);
    public static bool GetIsEnabled(DependencyObject element) => (bool)element.GetValue(IsEnabledProperty);

    private static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not FrameworkElement element || args.NewValue is not true)
            return;

        element.IsVisibleChanged += (_, _) => Animate(element);
        element.Loaded += (_, _) => Animate(element);
    }

    private static void Animate(FrameworkElement element)
    {
        if (!element.IsVisible)
            return;

        element.BeginAnimation(
            UIElement.OpacityProperty,
            new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(220))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            });
    }
}

public static class DataGridColumns
{
    public static readonly DependencyProperty ParameterHeaderProperty = Register(nameof(ParameterHeaderProperty));
    public static readonly DependencyProperty ValueHeaderProperty = Register(nameof(ValueHeaderProperty));
    public static readonly DependencyProperty TimeHeaderProperty = Register(nameof(TimeHeaderProperty));
    public static readonly DependencyProperty CostHeaderProperty = Register(nameof(CostHeaderProperty));

    public static void SetParameterHeader(DependencyObject element, string? value) => element.SetValue(ParameterHeaderProperty, value);
    public static string? GetParameterHeader(DependencyObject element) => (string?)element.GetValue(ParameterHeaderProperty);
    public static void SetValueHeader(DependencyObject element, string? value) => element.SetValue(ValueHeaderProperty, value);
    public static string? GetValueHeader(DependencyObject element) => (string?)element.GetValue(ValueHeaderProperty);
    public static void SetTimeHeader(DependencyObject element, string? value) => element.SetValue(TimeHeaderProperty, value);
    public static string? GetTimeHeader(DependencyObject element) => (string?)element.GetValue(TimeHeaderProperty);
    public static void SetCostHeader(DependencyObject element, string? value) => element.SetValue(CostHeaderProperty, value);
    public static string? GetCostHeader(DependencyObject element) => (string?)element.GetValue(CostHeaderProperty);

    private static DependencyProperty Register(string name) =>
        DependencyProperty.RegisterAttached(name.Replace("Property", "", StringComparison.Ordinal), typeof(string), typeof(DataGridColumns), new PropertyMetadata(null, OnChanged));

    private static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not System.Windows.Controls.DataGrid grid)
            return;

        if (grid.IsLoaded)
            Apply(grid);
        else
            grid.Loaded += (_, _) => Apply(grid);
    }

    private static void Apply(System.Windows.Controls.DataGrid grid)
    {
        if (grid.Columns.Count < 5)
            return;

        grid.Columns[1].Header = GetParameterHeader(grid);
        grid.Columns[2].Header = GetValueHeader(grid);
        grid.Columns[3].Header = GetTimeHeader(grid);
        grid.Columns[4].Header = GetCostHeader(grid);
    }
}
