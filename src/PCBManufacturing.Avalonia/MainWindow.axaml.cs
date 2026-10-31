using Avalonia;
using Avalonia.Controls;

namespace PCBManufacturing.AvaloniaApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += (_, _) => CenterOnPrimaryScreen();
    }

    private void CenterOnPrimaryScreen()
    {
        var area = Screens?.Primary?.WorkingArea;
        if (area is null)
            return;

        var width = (int)Math.Min(Bounds.Width, area.Value.Width);
        var height = (int)Math.Min(Bounds.Height, area.Value.Height);
        Position = new PixelPoint(
            area.Value.X + Math.Max(0, (area.Value.Width - width) / 2),
            area.Value.Y + Math.Max(0, (area.Value.Height - height) / 2));
    }
}
