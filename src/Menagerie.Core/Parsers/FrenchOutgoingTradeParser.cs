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
        new List<Token<OutgoingTrade>>
        {
            new("@", false),
            new(" Salut, je voudrais t'acheter ", true, (e, v) => e.Player = v, typeof(string)),
            new(" contre ", true, (e, v) => e.Item = new Item(v), typeof(string)),
            new(" (ligue ", true, (e, v) => e.Price = new Price(v), typeof(string)),
            new(").", true, (e, v) => e.League = v, typeof(string)),
        }
    )
    {
    }

    #endregion
}