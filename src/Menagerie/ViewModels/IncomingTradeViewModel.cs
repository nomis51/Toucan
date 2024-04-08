using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.Transformation;
using Menagerie.Core.Enums;
using Menagerie.Core.Models.Trading;
using Menagerie.Core.Services;
using Menagerie.Enums;
using Menagerie.Helpers;
using Menagerie.Models.IncomingTradeViewModels;
using Menagerie.Services;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class IncomingTradeViewModel : ViewModelBase
{
    #region Events

    public delegate void RemovedEvent(Guid id);

    public event RemovedEvent? OnRemoved;

    #endregion

    #region Props

    public Trade Trade { get; }
    public Task<Bitmap?> CurrencyImage => ImageHelper.LoadFromWeb(Trade.Price.CurrencyImageUrl);

    public int PriceQuantityFontSize
    {
        get
        {
            var str = Trade.Price.Value.ToString(CultureInfo.InvariantCulture);
            var hasDot = str.Contains('.', StringComparison.Ordinal);
            str = str.Replace(".", string.Empty);

            return str.Length switch
            {
                1 => 28,
                2 when !hasDot => 22,
                2 => 21,
                3 when !hasDot => 17,
                3 => 16,
                4 when !hasDot => 15,
                4 => 13,
                _ => 1
            };
        }
    }

    public int PriceQuantityColspan => PriceQuantityFontSize <= 15 ? 2 : 1;
    public int PriceQuantityColumn => PriceQuantityFontSize <= 15 ? 0 : 1;

    public HorizontalAlignment PriceQuantityHorizontalAlignment =>
        PriceQuantityFontSize <= 15 ? HorizontalAlignment.Right : HorizontalAlignment.Center;

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

    public int Size;

    #endregion

    #region Constructors

    public IncomingTradeViewModel(Trade trade, int size)
    {
        Trade = trade;
        Size = size;

        GenerateTooltip();

        AppService.OnPlayerJoined += Events_OnPlayerJoined;
        AppService.OnTradeAccepted += Events_OnTradeAccepted;
        AppService.OnTradeCancelled += Events_OnTradeCancelled;
    }

    #endregion

    #region Public methods

    public void DoNextAction()
    {
        AudioService.Instance.PlayEffect(AudioEffect.Click);
        
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
        AudioService.Instance.PlayEffect(AudioEffect.Click);
        
        Trade.State &= ~TradeState.Initial;
        Trade.State |= TradeState.Busy;
        this.RaisePropertyChanged(nameof(BorderBrush));

        AppService.Instance.SendBusyWhisper(Trade);
    }

    public void Whisper()
    {
        AudioService.Instance.PlayEffect(AudioEffect.Click);
        
        AppService.Instance.PrepareToSendWhisper(Trade);
    }

    public void SaySold()
    {
        AudioService.Instance.PlayEffect(AudioEffect.Click);
        
        AppService.Instance.SendSoldWhisper(Trade);
        SendDenyOffer();
    }

    public void AskStillInterested()
    {
        AudioService.Instance.PlayEffect(AudioEffect.Click);
        
        Trade.State &= ~TradeState.Initial;
        Trade.State |= TradeState.StillInterested;
        this.RaisePropertyChanged(nameof(BorderBrush));

        AppService.Instance.SendStillInterestedWhisper(Trade);
    }

    public void SendInvitePlayer()
    {
        AudioService.Instance.PlayEffect(AudioEffect.Click);
        
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
        AudioService.Instance.PlayEffect(AudioEffect.Click);
        
        if (Trade.State.HasFlag(TradeState.PlayerInvited))
        {
            AppService.Instance.SendKickCommand(Trade);
        }

        Trade.State = TradeState.Done;
        this.RaisePropertyChanged(nameof(BorderBrush));

        OnRemoved?.Invoke(Trade.Id);
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

        AppService.Instance.SendThanksWhisper(Trade, true);
        SendDenyOffer();
    }

    private void Events_OnPlayerJoined(string player)
    {
        if (IsPlayerInTheArea) return;
        if (Trade.Player != player || Trade.State.HasFlag(TradeState.PlayerJoined)) return;

        AudioService.Instance.PlayEffect(AudioEffect.PlayerJoined);
        
        Trade.State &= ~TradeState.StillInterested;
        Trade.State |= TradeState.PlayerJoined;
        IsPlayerInTheArea = true;
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