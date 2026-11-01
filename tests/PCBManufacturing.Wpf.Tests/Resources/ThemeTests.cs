using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace PCBManufacturing.Wpf.Tests.Resources;

public sealed class ThemeTests
{
    private static readonly string[] ThemeBrushes =
    [
        "Brush.App",
        "Brush.Card",
        "Brush.CardAlt",
        "Brush.Stroke",
        "Brush.TextPrimary",
        "Brush.TextSecondary",
        "Brush.Accent",
        "Brush.AccentHover",
        "Brush.AccentPressed",
        "Brush.AccentSubtle",
        "Brush.OnAccent",
        "Brush.Input",
        "Brush.Hover",
        "Brush.Error",
        "Brush.ErrorSubtle",
        "Brush.Success",
        "Brush.SuccessSubtle",
        "Brush.Preview",
        "Brush.ScrollTrack",
        "Brush.ScrollThumb",
        "Brush.ScrollThumbHover",
        "Brush.ScrollThumbPressed"
    ];

    [Theory]
    [InlineData("Light.xaml")]
    [InlineData("Dark.xaml")]
    public void Dictionary_DefinesTheWindows11Palette(string fileName)
    {
        StaThread.Run(() =>
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Resources", fileName);
            using var stream = File.OpenRead(path);
            var dictionary = Assert.IsType<ResourceDictionary>(XamlReader.Load(stream));
            foreach (var key in ThemeBrushes)
                Assert.IsType<SolidColorBrush>(dictionary[key]);
        });
    }

    [Fact]
    public void FluentDictionary_ReplacesClassicChrome()
    {
        StaThread.Run(() =>
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Fluent.xaml");
            using var stream = File.OpenRead(path);
            var dictionary = Assert.IsType<ResourceDictionary>(XamlReader.Load(stream));
            Assert.IsType<Style>(dictionary[typeof(ScrollBar)]);
            Assert.IsType<Style>(dictionary[typeof(ScrollViewer)]);
            Assert.IsType<Style>(dictionary[typeof(ProgressBar)]);
            Assert.IsType<Style>(dictionary[typeof(Separator)]);
        });
    }

    [Fact]
    public void FluentScrollBar_MovesTheScrollViewer()
    {
        StaThread.Run(() =>
        {
            var window = new Window { Width = 220, Height = 180 };
            window.Resources.MergedDictionaries.Add(LoadDictionary("Light.xaml"));
            window.Resources.MergedDictionaries.Add(LoadDictionary("Fluent.xaml"));

            var viewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
                Content = new Border { Width = 400, Height = 800, Background = Brushes.White }
            };
            window.Content = viewer;
            window.Show();
            viewer.UpdateLayout();
            Drain(viewer);

            var vertical = FindBar(viewer, Orientation.Vertical);
            var horizontal = FindBar(viewer, Orientation.Horizontal);
            Assert.NotNull(vertical);
            Assert.NotNull(horizontal);
            Assert.True(vertical.Maximum > 0);
            Assert.Equal(14, vertical.ActualWidth);
            Assert.Equal(14, horizontal.ActualHeight);
            Assert.NotNull(vertical.Track);
            Assert.NotNull(horizontal.Track);

            vertical.SmallChange = vertical.Maximum;
            ScrollBar.LineDownCommand.Execute(null, vertical);
            Drain(viewer);
            Assert.True(viewer.VerticalOffset > 0);

            horizontal.SmallChange = horizontal.Maximum;
            ScrollBar.LineRightCommand.Execute(null, horizontal);
            Drain(viewer);
            Assert.True(viewer.HorizontalOffset > 0);

            window.Close();
        });
    }

    private static ResourceDictionary LoadDictionary(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", fileName);
        using var stream = File.OpenRead(path);
        return Assert.IsType<ResourceDictionary>(XamlReader.Load(stream));
    }

    private static void Drain(DispatcherObject source)
    {
        source.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
    }

    private static ScrollBar? FindBar(DependencyObject root, Orientation orientation)
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is ScrollBar bar && bar.Orientation == orientation)
                return bar;
            var nested = FindBar(child, orientation);
            if (nested is not null)
                return nested;
        }

        return null;
    }
}
