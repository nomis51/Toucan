using System.Runtime.InteropServices;
using Avalonia.Interactivity;
using Menagerie.Core.WinApi;
using Menagerie.ViewModels;

namespace Menagerie.Windows;

public partial class MainWindow : WindowBase<MainWindowViewModel>
{
    #region Windows

    private readonly IncomingTradesWindow _incomingTradesWindow = new()
    {
        DataContext = new IncomingTradesWindowViewModel()
    };

    #endregion

    #region Constructors

    public MainWindow()
    {
        InitializeComponent();
    }

    #endregion

    #region Private methods

    private void AdjustWindow()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        var hwnd = GetTopLevel(this)!.TryGetPlatformHandle()!.Handle;
        var windowStyle = User32.GetWindowLong(hwnd, User32.GWL_EX_STYLE);
        windowStyle |= User32.WS_EX_TOOLWINDOW;
        _ = User32.SetWindowLong(hwnd, User32.GWL_EX_STYLE, windowStyle);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        ViewModel!.OnOverlayVisibilityChangedEvent += OnOverlayVisibilityChangedEvent;

        AdjustWindow();

        _incomingTradesWindow.Show(this);
    }

    private void OnOverlayVisibilityChangedEvent(bool isVisible)
    {
        InvokeUi(() =>
        {
            if (isVisible)
            {
                _incomingTradesWindow.Show(this);
            }
            else
            {
                _incomingTradesWindow.Hide();
            }
        });
    }

    #endregion
}