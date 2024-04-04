using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Menagerie.Core.Enums;
using Menagerie.Core.Models.Trading;
using Menagerie.Core.Services;
using Menagerie.Models.IncomingTradeViewModels;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class IncomingTradeViewModel : ViewModelBase
{
     #region Events

    public delegate void RemovedEvent(Guid id);

    public event RemovedEvent? Removed;

    #endregion

    #region Props

    public Trade Trade { get; }

    public int PriceQuantityFontSize
    {
        get
        {
            var str = Trade.Price.Value.ToString(CultureInfo.InvariantCulture);
            var hasDot = str.Contains('.', StringComparison.Ordinal);
            str = str.Replace(".", string.Empty);

            return str.Length switch
            {
                1 => 24,
                2 when !hasDot => 22,
                2 => 21,
                3 when !hasDot => 18,
                3 => 17,
                4 when !hasDot => 15,
                4 => 13,
                _ => 1
            };
        }
    }

    public ITransform? PriceQuantityTransform => PriceQuantityFontSize <= 13 ? TransformOperations.Parse("rotate(-35deg)") : null;

    public IBrush BorderBrush
    {
        get
        {
            if (Trade.State.HasFlag(TradeState.Done)) return new SolidColorBrush((Color)Application.Current!.Resources["ErrorColor"]!);
            if (Trade.State.HasFlag(TradeState.Trading)) return new SolidColorBrush((Color)Application.Current!.Resources["WarningColor"]!);
            if (Trade.State.HasFlag(TradeState.PlayerInvited)) return new SolidColorBrush((Color)Application.Current!.Resources["SuccessColor"]!);
            if (Trade.State.HasFlag(TradeState.Busy)) return new SolidColorBrush((Color)Application.Current!.Resources["AccentColor"]!);

            return new SolidColorBrush((Color)Application.Current!.Resources["Background0"]!);
        }
    }

    public ObservableCollection<TooltipLine> TooltipLines { get; private set; } = new();
    public bool CanSayBusy => !Trade.State.HasFlag(TradeState.PlayerInvited);

    private bool _isPlayerInTheArea;

    public bool IsPlayerInTheArea
    {
        get => _isPlayerInTheArea;
        set => this.RaiseAndSetIfChanged(ref _isPlayerInTheArea, value);
    }

    public int OfferSize;

    #endregion

    #region Constructors

    public IncomingTradeViewModel(Trade trade, int size)
    {
        Trade = trade;
        OfferSize = size;

        GenerateTooltip();

        AppService.PlayerJoined += Events_OnPlayerJoined;
        AppService.TradeAccepted += Events_OnTradeAccepted;
        AppService.TradeCancelled += Events_OnTradeCancelled;
    }

    #endregion

    #region Public methods

    public void DoNextAction()
    {
        if (!Trade.State.HasFlag(TradeState.PlayerInvited))
        {
            SendInvitePlayer();
            this.RaisePropertyChanged(nameof(CanSayBusy));
        }
        else if (!Trade.State.HasFlag(TradeState.Trading))
        {
            SendTrade();
            this.RaisePropertyChanged(nameof(CanSayBusy));
        }
    }

    public void SayBusy()
    {
        Trade.State &= ~TradeState.Initial;
        Trade.State |= TradeState.Busy;
        this.RaisePropertyChanged(nameof(BorderBrush));

        AppService.Instance.SendBusyWhisper(Trade);
    }

    public void Whisper()
    {
        AppService.Instance.PrepareToSendWhisper(Trade);
    }

    public void SaySold()
    {
        AppService.Instance.SendSoldWhisper(Trade);
        SendDenyOffer();
    }

    public void AskStillInterested()
    {
        Trade.State &= ~TradeState.Initial;
        Trade.State |= TradeState.StillInterested;
        this.RaisePropertyChanged(nameof(BorderBrush));

        AppService.Instance.SendStillInterestedWhisper(Trade);
    }

    public void SendInvitePlayer()
    {
        Trade.State &= ~TradeState.Initial;

        if (!Trade.State.HasFlag(TradeState.PlayerInvited))
        {
            Trade.State |= TradeState.PlayerInvited;
            this.RaisePropertyChanged(nameof(BorderBrush));

            AppService.Instance.SendInviteCommand(Trade);
        }
        else
        {
            AppService.Instance.SendReInviteCommand(Trade);
        }
    }

    public void SendDenyOffer()
    {
        Trade.State = TradeState.Done;
        this.RaisePropertyChanged(nameof(BorderBrush));

        if (Trade.State.HasFlag(TradeState.PlayerInvited))
        {
            AppService.Instance.SendKickCommand(Trade);
        }

        Removed?.Invoke(Trade.Id);
    }

    #endregion

    #region Private methdos

    private void SendTrade()
    {
        Trade.State &= ~TradeState.StillInterested;
        Trade.State |= TradeState.Trading;
        this.RaisePropertyChanged(nameof(BorderBrush));

        AppService.Instance.SendTradeRequestCommand(Trade);
    }

    private void Events_OnTradeCancelled()
    {
        if (!Trade.State.HasFlag(TradeState.Trading)) return;

        Trade.State &= ~TradeState.Trading;
        this.RaisePropertyChanged(nameof(BorderBrush));
    }

    private void Events_OnTradeAccepted()
    {
        if (!Trade.State.HasFlag(TradeState.Trading)) return;

        AppService.Instance.SendThanksWhisper(Trade);
        SendDenyOffer();
    }

    private void Events_OnPlayerJoined(string player)
    {
        if (IsPlayerInTheArea) return;
        if (Trade.Player != player || Trade.State.HasFlag(TradeState.PlayerJoined)) return;

        Trade.State &= ~TradeState.StillInterested;
        Trade.State |= TradeState.PlayerJoined;
        this.RaisePropertyChanged(nameof(BorderBrush));
    }

    private void GenerateTooltip()
    {
        TooltipLines.Add(new TooltipLine("Time : ", Trade.Time.ToLongTimeString()));
        TooltipLines.Add(new TooltipLine("Player : ", Trade.Player));
        TooltipLines.Add(new TooltipLine("Item : ", $"{(Trade.Item.Quantity > 0 ? $"{Trade.Item.Quantity}x " : string.Empty)}{Trade.Item.Name}"));
        TooltipLines.Add(new TooltipLine("Price : ", $"{Trade.Price.Value} {Trade.Price.Currency}"));
        TooltipLines.Add(new TooltipLine("League : ", Trade.League));
        TooltipLines.Add(new TooltipLine("Location : ", $"{Trade.Location.StashTab} (Left: {Trade.Location.Left}, Top: {Trade.Location.Top})"));
    }

    #endregion
}