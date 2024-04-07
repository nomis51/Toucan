using System.Text.RegularExpressions;
using Menagerie.Core.Models.Events;
using Menagerie.Core.Models.Parsing;

namespace Menagerie.Core.Parsers;

public class TradeAcceptedParser : Parser<TradeAcceptedEvent>
{
    #region Constructors

    public TradeAcceptedParser() : base(
        new Regex("Trade accepted", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>()
    )
    {
    }

    #endregion
}