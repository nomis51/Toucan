using System.Text.RegularExpressions;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class TextParserService : ITextParserService
{
    #region Members

    private readonly IncomingOfferParser _incomingOfferParser;
    private readonly OutgoingOfferParser _outgoingOfferParser;
    private readonly TradeAcceptedParser _tradeAcceptedParser;
    private readonly TradeCancelledParser _tradeCancelledParser;
    private readonly PlayerJoinedParser _playerJoinedParser;
    private readonly RussianOutgoingOfferParser _russianOutgoingOfferParser;
    private readonly KoreanOutgoingOfferParser _koreanOutgoingOfferParser;
    private readonly FrenchOutgoingOfferParser _frenchOutgoingOfferParser;
    private readonly GermanOutgoingOfferParser _germanOutgoingOfferParser;
    private readonly LocationParser _locationParser;
    
    #endregion
    
    #region Public methods

    public void ParseClientFileLine(string line)
    {
    }

    public void ParseClipboardLine(string line)
    {
    }

    #endregion
}