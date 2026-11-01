using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;

namespace PCBManufacturing.AvaloniaApp;

public partial class MainWindow : Window
{
    private const int WmSetIcon = 0x0080;
    private const uint WsExDlgModalFrame = 0x0001;
    private const uint FrameChanged = 0x0027;
    private static readonly nint InvisibleIcon = CreateInvisibleIcon();

    public MainWindow()
    {
        InitializeComponent();
        Win32Properties.AddWindowStylesCallback(this, static (style, exStyle) => (style, exStyle | WsExDlgModalFrame));
        Win32Properties.AddWndProcHookCallback(this, static (hWnd, msg, wParam, lParam, ref handled) =>
        {
            if (msg == WmSetIcon && wParam is 0 or 2 && lParam != InvisibleIcon)
            {
                handled = true;
                SendMessage(hWnd, WmSetIcon, wParam, InvisibleIcon);
            }

            return 0;
        });
        Opened += (_, _) =>
        {
            CenterOnPrimaryScreen();
            RefreshCaption();
        };
    }

    private void RefreshCaption()
    {
        if (!OperatingSystem.IsWindows())
            return;

        var handle = TryGetPlatformHandle()?.Handle ?? 0;
        if (handle == 0)
            return;

        var style = GetWindowLongPtr(handle, -20);
        SetWindowLongPtr(handle, -20, style | (nint)WsExDlgModalFrame);
        var path = Environment.ProcessPath;
        if (!string.IsNullOrEmpty(path) && ExtractIconEx(path, 0, out var large, out _, 1) > 0 && large != 0)
            SendMessage(handle, WmSetIcon, 1, large);

        SendMessage(handle, WmSetIcon, 0, InvisibleIcon);
        SendMessage(handle, WmSetIcon, 2, InvisibleIcon);
        SetWindowPos(handle, 0, 0, 0, 0, 0, FrameChanged);
    }

    private static nint CreateInvisibleIcon()
    {
        var mask = new byte[32];
        Array.Fill(mask, (byte)0xFF);
        var color = new byte[32];
        return CreateIcon(0, 16, 16, 1, 1, mask, color);
    }

    [DllImport("user32.dll", EntryPoint = "SendMessageW")]
    private static extern nint SendMessage(nint hWnd, int msg, nint wParam, nint lParam);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    private static extern nint GetWindowLongPtr(nint hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
    private static extern nint SetWindowLongPtr(nint hWnd, int nIndex, nint newLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int width, int height, uint flags);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint CreateIcon(nint instance, int width, int height, byte planes, byte bitsPerPixel, byte[] andMask, byte[] xorMask);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern uint ExtractIconEx(string file, int index, out nint largeIcon, out nint smallIcon, uint count);

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
