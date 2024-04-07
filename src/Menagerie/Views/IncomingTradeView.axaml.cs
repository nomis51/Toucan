using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using Menagerie.Core.Helpers;
using Menagerie.ViewModels;

namespace Menagerie.Views;

public partial class IncomingTradeView : ViewBase<IncomingTradeViewModel>
{
    #region Constructors

    public IncomingTradeView()
    {
        InitializeComponent();
    }

    #endregion

    #region Private methods

    private void Border_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        Dispatch(vm =>
        {
            if (KeyboardHelper.State.Control && KeyboardHelper.State.Shift)
            {
                vm?.AskStillInterested();
            }
            else if (KeyboardHelper.State.Control)
            {
                vm?.SaySold();
            }
            else if (KeyboardHelper.State.Shift)
            {
                vm?.Whisper();
            }
            else
            {
                vm?.DoNextAction();
            }
        });
    }

    private void ButtonBusy_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatch(vm => vm?.SayBusy());
    }

    private void ButtonReInvitePlayer_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatch(vm => vm?.SendInvitePlayer());
    }

    private void ButtonDenyOffer_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatch(vm => vm?.SendDenyOffer());
    }

    private void OnInitialized(object? sender, EventArgs e)
    {
        Width = ViewModel!.Size;
        Height = ViewModel!.Size * 1.1;
    }

    #endregion
}