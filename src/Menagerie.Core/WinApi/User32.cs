using System.Runtime.InteropServices;

namespace Menagerie.Core.WinApi;

public class User32
{
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hwnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(IntPtr hWnd, int flags);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

    public const int GWL_EX_STYLE = -20;
    public const int WS_EX_APPWINDOW = 0x00040000;
    public const int WS_EX_TOOLWINDOW = 0x00000080;
    public const int WS_EX_TRANSPARENT = 0x00000020;
    public const long WS_EX_DLGMODALFRAME = 0x00000001L;
    public const long WS_EX_CLIENTEDGE = 0x02000000L;
    public const long WS_EX_STATICEDGE = 0x00020000L;

    public const int GWL_STYLE = -16;
    public const long WS_CAPTION = 0x00C00000L;
    public const long WS_SYSMENU = 0x00080000L;
    public const long WS_THICKFRAME = 0x00040000L;
    public const long WS_MINIMIZEBOX = 0x00020000L;
    public const long WS_MAXIMIZEBOX = 0x00010000L;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int MessageBox(IntPtr handle, string message, string title, long type);
}