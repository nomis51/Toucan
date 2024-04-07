using System.Text.RegularExpressions;
using Menagerie.Core.Models.Events;
using Menagerie.Core.Models.Parsing;

namespace Menagerie.Core.Parsers;

public class TradeCancelledParser : Parser<TradeCancelledEvent>
{
    #region Constructors

    public TradeCancelledParser() : base(
        new Regex("Trade cancelled", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token<TradeCancelledEvent>>()
    )
    {
    }

    #endregion
}