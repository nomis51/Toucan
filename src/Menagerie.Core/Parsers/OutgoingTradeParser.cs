using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing;
using Menagerie.Core.Models.Trading;

namespace Menagerie.Core.Parsers;

public class OutgoingTradeParser : Parser<OutgoingTrade>
{
    #region Constructors

    public OutgoingTradeParser() : base(
        new Regex("@To .+: Hi, (I would|I'd) like to buy your .+ (listed for|for my) [0-9\\.]+ .+ in .+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token<OutgoingTrade>>
        {
            new("@To ", false),
            new("<", false, breakOnFail: false),
            new("> ", false, breakOnFail: false),
            new(": Hi, ", true, (e, v) => e.Player = v, typeof(string)),
            new("I would", false, (e, v) => { }, null, "I'd"),
            new(" like to buy your ", false),
            new(" listed for ", true, (e, v) => e.Item = new Item(v), typeof(string), " for my "),
            new(" in ", true, (e, v) => e.Price = new Price(v), typeof(double)),
            new(".", true, (e, v) => e.League = v, typeof(string), endOfString: true)
        }
    )
    {
    }

    #endregion
}