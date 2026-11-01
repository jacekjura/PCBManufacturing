using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PCBManufacturing.Wpf;

public static class AppIcons
{
    public static ImageSource Window { get; } = Load();

    private static ImageSource Load()
    {
        var decoder = BitmapDecoder.Create(
            new Uri("pack://application:,,,/Assets/AppIcon.ico", UriKind.Absolute),
            BitmapCreateOptions.PreservePixelFormat,
            BitmapCacheOption.OnLoad);
        var frame = decoder.Frames.OrderByDescending(item => item.PixelWidth).First();
        var image = frame.Format == PixelFormats.Pbgra32
            ? (BitmapSource)frame
            : new FormatConvertedBitmap(frame, PixelFormats.Pbgra32, null, 0);
        image.Freeze();
        return image;
    }
}
