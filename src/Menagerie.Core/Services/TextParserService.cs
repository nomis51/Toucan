using Menagerie.Core.Parsers;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class TextParserService : ITextParserService
{
    #region Members

    private readonly IncomingTradeParser _incomingTradeParser = new();
    private readonly OutgoingTradeParser _outgoingTradeParser = new();
    private readonly TradeAcceptedParser _tradeAcceptedParser = new();
    private readonly TradeCancelledParser _tradeCancelledParser = new();
    private readonly PlayerJoinedParser _playerJoinedParser = new();
    private readonly RussianOutgoingTradeParser _russianOutgoingTradeParser = new();
    private readonly KoreanOutgoingTradeParser _koreanOutgoingTradeParser = new();
    private readonly FrenchOutgoingTradeParser _frenchOutgoingTradeParser = new();
    private readonly GermanOutgoingTradeParser _germanOutgoingTradeParser = new();
    private readonly LocationParser _locationParser = new();

    #endregion

    #region Public methods

    public void ParseClientFileLine(string line)
    {
        if (_locationParser.CanParse(line))
        {
            var location = _locationParser.Parse(line);
            if (location is null) return;

            AppService.LocationUpdatedEventInvoke(location.Location);

            return;
        }

        if (_incomingTradeParser.CanParse(line))
        {
            var trade = _incomingTradeParser.Parse(line);
            if (trade is null) return;

            trade.Whisper = line;
            AppService.Instance.NewIncomingTradeReceived(trade);
            return;
        }

        if (_outgoingTradeParser.CanParse(line))
        {
            var offer = _outgoingTradeParser.Parse(line);
            if (offer is null) return;

            offer.Whisper = line;
            AppService.Instance.NewOutgoingTradeReceived(offer);
            return;
        }

        if (_tradeAcceptedParser.CanParse(line))
        {
            _ = _tradeAcceptedParser.Parse(line);
            AppService.TradeAcceptedEventInvoke();
            return;
        }

        if (_tradeCancelledParser.CanParse(line))
        {
            _ = _tradeCancelledParser.Parse(line);
            AppService.TradeCancelledEventInvoke();
        }

        if (_playerJoinedParser.CanParse(line))
        {
            var playerJoinedEvent = _playerJoinedParser.Parse(line);
            if (playerJoinedEvent is null) return;
            AppService.PlayerJoinedEventInvoke(playerJoinedEvent.Player);
        }
    }

    public void ParseClipboardLine(string line)
    {
    }

    #endregion
}