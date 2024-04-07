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
        new List<Token>
        {
            new("@", false),
            new(" Hi, ich möchte dein ", true, "Player", typeof(string)),
            new(" für mein ", true, "ItemName", typeof(string)),
            new(" in der ", true, "PriceStr", typeof(string)),
            new("-Liga kaufen.", true, "League", typeof(string)),
        }
    )
    {
    }

    #endregion
}