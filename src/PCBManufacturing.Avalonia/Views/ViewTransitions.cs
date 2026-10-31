using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Styling;

namespace PCBManufacturing.AvaloniaApp.Views;

internal static class ViewTransitions
{
    public static Task FadeInAsync(Control control)
    {
        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(220),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters = { new Setter(Visual.OpacityProperty, 0d) }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters = { new Setter(Visual.OpacityProperty, 1d) }
                }
            }
        };
        return animation.RunAsync(control);
    }

    public static Task PulseAsync(Control control)
    {
        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(280),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters = { new Setter(Visual.OpacityProperty, 0.45d) }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters = { new Setter(Visual.OpacityProperty, 1d) }
                }
            }
        };
        return animation.RunAsync(control);
    }
}
