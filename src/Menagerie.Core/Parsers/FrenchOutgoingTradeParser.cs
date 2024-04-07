using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing;
using Menagerie.Core.Models.Trading;

namespace Menagerie.Core.Parsers;

public class FrenchOutgoingTradeParser : Parser<OutgoingTrade>
{
    #region Constructors

    public FrenchOutgoingTradeParser() : base(
        new Regex("@.+ Salut, je voudrais t'acheter .+ contre [0-9\\.]+ .+ \\(ligue .+\\)\\.",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>
        {
            new("@", false),
            new(" Salut, je voudrais t'acheter ", true, "Player", typeof(string)),
            new(" contre ", true, "ItemName", typeof(string)),
            new(" (ligue ", true, "PriceStr", typeof(string)),
            new(").", true, "League", typeof(string)),
        }
    )
    {
    }

    #endregion
}