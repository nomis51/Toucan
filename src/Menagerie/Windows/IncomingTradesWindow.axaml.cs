using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Menagerie.Core.Services;
using Menagerie.Core.WinApi;
using Menagerie.ViewModels;

namespace Menagerie.Windows;

public partial class IncomingTradesWindow : WindowBase<IncomingTradesWindowViewModel>
{
    #region Constructors

    public IncomingTradesWindow()
    {
        InitializeComponent();
    }

    #endregion

    #region Private methods

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        AppService.Instance.AddOverlayWindowHandle(GetTopLevel(this)!.TryGetPlatformHandle()!.Handle);
        AdjustPosition();
    }

    private void AdjustPosition()
    {
        AdjustWindow();

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Left hud and right hud are using about 29% of the screen width
            // which gives the exp bar portion about 41% of the screen width
            // the exp bar is using about 2.5% of the screen height
            // the space above the exp bar is about 11% of the screen height
            // which gives about 7.5% of the screen height space for the panel

            var size = desktop.MainWindow!.Screens.Primary!.Bounds.Size;
            Position = new PixelPoint((int)(size.Width * .278), (int)(size.Height * .962 - Height));
            Height = (int)(size.Height * .092);
            Width = (int)(size.Width * .444);
            MaxWidth = (int)(size.Width * .88);
            ViewModel?.SetTradeSize((int)(size.Height * .092 - ButtonRemoveAllTrades.Height));
            // Panel.Width = 0;
        }
    }

    private void ScrollViewer_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        switch (e.Delta.Y)
        {
            case > 0:
                ScrollViewer.PageLeft();
                break;

            case < 0:
                ScrollViewer.PageRight();
                break;
        }
    }

    private void AdjustWindow()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        var hwnd = GetTopLevel(this)!.TryGetPlatformHandle()!.Handle;

        var windowStyle = User32.GetWindowLong(hwnd, User32.GWL_STYLE);
        windowStyle &= ~(User32.WS_CAPTION | User32.WS_THICKFRAME | User32.WS_MINIMIZEBOX | User32.WS_MAXIMIZEBOX | User32.WS_SYSMENU);
        _ = User32.SetWindowLong(hwnd, User32.GWL_STYLE, windowStyle);

        var windowExStyle = User32.GetWindowLong(hwnd, User32.GWL_EX_STYLE);
        windowExStyle &= ~(User32.WS_EX_DLGMODALFRAME | User32.WS_EX_CLIENTEDGE | User32.WS_EX_STATICEDGE);
        _ = User32.SetWindowLong(hwnd, User32.GWL_EX_STYLE, windowExStyle);
    }


    private void ButtonRemoveAllTrades_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel?.RemoveAllTrades();
    }
   
    #endregion
}