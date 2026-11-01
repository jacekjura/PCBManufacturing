using System.Windows;
using System.Windows.Media.Animation;

namespace PCBManufacturing.Wpf.Behaviors;

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
