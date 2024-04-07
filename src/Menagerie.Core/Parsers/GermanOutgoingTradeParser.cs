using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing;
using Menagerie.Core.Models.Trading;

namespace Menagerie.Core.Parsers;

public class GermanOutgoingTradeParser : Parser<OutgoingTrade>
{
    #region Constructors

    public GermanOutgoingTradeParser() : base(
        new Regex("@.+ Hi, ich möchte dein .+ für mein [0-9\\.]+ .+ in der .+\\-Liga kaufen\\.",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token<OutgoingTrade>>
        {
            new("@", false),
            new(" Hi, ich möchte dein ", true, (e, v) => e.Player = v, typeof(string)),
            new(" für mein ", true, (e, v) => e.Item = new Item(v), typeof(string)),
            new(" in der ", true, (e, v) => e.Price = new Price(v), typeof(string)),
            new("-Liga kaufen.", true, (e, v) => e.League = v, typeof(string)),
        }
    )
    {
    }

    #endregion
}