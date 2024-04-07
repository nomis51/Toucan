using Menagerie.Core.Helpers;
using Menagerie.Core.Models.Setting;
using Menagerie.Core.Models.Trading;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class AppService
{
    #region Singleton

    private static readonly object LockInstance = new();
#pragma warning disable CS8618
    private static AppService _instance;
#pragma warning restore CS8618

    public static AppService Instance
    {
        get
        {
            lock (LockInstance)
            {
                // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
                _instance ??= new AppService();
            }

            return _instance;
        }
    }

    #endregion

    #region Events

    #region TradeAccepted

    public delegate void TradeAcceptedEvent();

    public static event TradeAcceptedEvent? OnTradeAccepted;
    public static void TradeAcceptedEventInvoke() => OnTradeAccepted?.Invoke();

    #endregion

    #region TradeCancelled

    public delegate void TradeCancelledEvent();

    public static event TradeCancelledEvent? OnTradeCancelled;
    public static void TradeCancelledEventInvoke() => OnTradeCancelled?.Invoke();

    #endregion

    #region PlayerJoined

    public delegate void PlayerJoinedEvent(string player);

    public static event PlayerJoinedEvent? OnPlayerJoined;
    public static void PlayerJoinedEventInvoke(string player) => OnPlayerJoined?.Invoke(player);

    #endregion

    #region NewIncomingOffer

    public delegate void NewIncomingTradeEvent(IncomingTrade trade);

    public static event NewIncomingTradeEvent? OnNewIncomingTrade;
    public static void NewIncomingTradeEventInvoke(IncomingTrade offer) => OnNewIncomingTrade?.Invoke(offer);

    #endregion

    #region NewOutgoingOffer

    public delegate void NewOutgoingTradeEvent(OutgoingTrade trade);

    public static event NewOutgoingTradeEvent? OnNewOutgoingTrade;
    public static void NewOutgoingTradeEventInvoke(OutgoingTrade offer) => OnNewOutgoingTrade?.Invoke(offer);

    #endregion

    #region LocationUpdated

    public delegate void LocationUpdatedEvent(string location);

    public static event LocationUpdatedEvent? OnLocationUpdated;
    public static void LocationUpdatedEventInvoke(string location) => OnLocationUpdated?.Invoke(location);

    #endregion

    #endregion

    #region Services

    private IFsService _fsService = null!;
    private IGameProcessService _gameProcessService = null!;
    private IClientFileService _clientFileService = null!;
    private ITextParserService _textParserService = null!;
    private ISettingsService _settingsService = null!;
    private IGameChatService _gameChatService = null!;

    #endregion

    #region Public methods

    public void Initialize(ServicesDependencies servicesDependencies)
    {
        _fsService = servicesDependencies.FsService;
        _gameProcessService = servicesDependencies.GameProcessService;
        _clientFileService = servicesDependencies.ClientFileService;
        _textParserService = servicesDependencies.TextParserService;
        _settingsService = servicesDependencies.SettingsService;
        _gameChatService = servicesDependencies.GameChatService;

        LogsHelper.Initialize();
        _ = _gameProcessService.FindProcess();
    }

    public Task<Settings> GetSettings()
    {
        return _settingsService.GetSettings();
    }

    public Task SaveSettings(Settings settings)
    {
        return _settingsService.SaveSettings(settings);
    }

    public string GetLogsFolder()
    {
        return _fsService.LogsFolder;
    }

    public string GetClientFilePath()
    {
        return _fsService.ClientFilePath;
    }

    public string GetAppFolder()
    {
        return _fsService.AppFolder;
    }

    public bool FocusGameWindow()
    {
        return _gameProcessService.FocusGameWindow();
    }

    public void GameProcessFound()
    {
        KeyboardHelper.Initialize(_gameProcessService.ProcessId);
        _fsService.GameProcessFound(_gameProcessService.ProcessLocation);
        _clientFileService.ClientFileLocationFound();
    }

    public void ClientFileLineFound(string line)
    {
        _textParserService.ParseClientFileLine(line);
    }

    public void NewIncomingTradeReceived(IncomingTrade trade)
    {
        trade.Price.CurrencyImage = ImageHelper.LoadFromWeb(CurrencyHelper.GetCurrencyImageLink(trade.Price.Currency));
        // TODO: price conversions
        NewIncomingTradeEventInvoke(trade);
    }

    public void NewOutgoingTradeReceived(OutgoingTrade trade)
    {
        trade.Price.CurrencyImage = ImageHelper.LoadFromWeb(CurrencyHelper.GetCurrencyImageLink(trade.Price.Currency));
        // TODO: price conversions

        NewOutgoingTradeEventInvoke(trade);
    }


    public void SendBusyWhisper(Trade trade)
    {
        _ = _gameChatService.SendBusyWhisper(trade);
    }

    public void PrepareToSendWhisper(Trade trade)
    {
        _ = _gameChatService.PrepareToSendWhisper(trade);
    }

    public void SendSoldWhisper(Trade trade)
    {
        _ = _gameChatService.SendSoldWhisper(trade);
    }

    public void SendStillInterestedWhisper(Trade trade)
    {
        _ = _gameChatService.SendStillInterestedWhisper(trade);
    }

    public void SendInviteCommand(Trade trade)
    {
        _ = _gameChatService.SendInvite(trade);
    }

    public void SendReInviteCommand(Trade trade)
    {
        _ = _gameChatService.SendReInvite(trade);
    }

    public void SendKickCommand(Trade trade)
    {
        _ = _gameChatService.SendKick(trade);
    }

    public void SendTradeRequestCommand(Trade trade)
    {
        _ = _gameChatService.SendTradeRequest(trade);
    }

    public void SendThanksWhisper(Trade trade)
    {
        _ = _gameChatService.SendThanksWhisper(trade);
    }

    #endregion
}