using Menagerie.Core.Helpers;
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

    public static event TradeAcceptedEvent? TradeAccepted;
    public static void TradeAcceptedEventInvoke() => TradeAccepted?.Invoke();

    #endregion

    #region TradeCancelled

    public delegate void TradeCancelledEvent();

    public static event TradeCancelledEvent? TradeCancelled;
    public static void TradeCancelledEventInvoke() => TradeCancelled?.Invoke();

    #endregion

    #region PlayerJoined

    public delegate void PlayerJoinedEvent(string player);

    public static event PlayerJoinedEvent? PlayerJoined;
    public static void PlayerJoinedEventInvoke(string player) => PlayerJoined?.Invoke(player);

    #endregion

    #region NewIncomingOffer

    public delegate void NewIncomingTradeEvent(IncomingTrade trade);

    public static event NewIncomingTradeEvent? NewIncomingTrade;
    public static void NewIncomingTradeEventInvoke(IncomingTrade offer) => NewIncomingTrade?.Invoke(offer);

    #endregion

    #region NewOutgoingOffer

    public delegate void NewOutgoingTradeEvent(OutgoingTrade trade);

    public static event NewOutgoingTradeEvent? NewOutgoingTrade;
    public static void NewOutgoingTradeEventInvoke(OutgoingTrade offer) => NewOutgoingTrade?.Invoke(offer);

    #endregion

    #region LocationUpdated

    public delegate void LocationUpdatedEvent(string location);

    public static event LocationUpdatedEvent? LocationUpdated;
    public static void LocationUpdatedEventInvoke(string location) => LocationUpdated?.Invoke(location);

    #endregion

    #endregion
    
    #region Services

    private IFsService _fsService = null!;
    private IGameProcessService _gameProcessService = null!;
    private IClientFileService _clientFileService = null!;
    private ITextParserService _textParserService = null!;

    #endregion

    #region Public methods

    public void Initialize(ServicesDependencies servicesDependencies)
    {
        _fsService = servicesDependencies.FsService;
        _gameProcessService = servicesDependencies.GameProcessService;
        _clientFileService = servicesDependencies.ClientFileService;
        _textParserService = servicesDependencies.TextParserService;
        
        LogsHelper.Initialize();
        // TODO: initialize keyboardHelper
    }

    public string GetLogsFolder()
    {
        return _fsService.LogsFolder;
    }

    public string GetClientFilePath()
    {
        return _fsService.ClientFilePath;
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

    public void SendBusyWhisper(Trade trade)
    {
        
    }

    public void PrepareToSendWhisper(Trade trade)
    {
        
    }

    public void SendSoldWhisper(Trade trade)
    {
        
    }

    public void SendStillInterestedWhisper(Trade trade)
    {
        
    }

    public void SendInviteCommand(Trade trade)
    {
        
    }

    public void SendReInviteCommand(Trade trade)
    {
        
    }

    public void SendKickCommand(Trade trade)
    {
        
    }

    public void SendTradeRequestCommand(Trade trade)
    {
        
    }

    public void SendThanksWhisper(Trade trade)
    {
        
    }
    
    #endregion
}