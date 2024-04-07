using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing;
using Menagerie.Core.Models.Trading;

namespace Menagerie.Core.Parsers;

public class IncomingTradeParser : Parser<IncomingTrade>
{
    #region Constructors

    public IncomingTradeParser() : base(
        new Regex("@From .+: Hi, (I would|I'd) like to buy your .+ (listed for|for my) [0-9\\.]+ .+ in .+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token<IncomingTrade>>
        {
            // (stash tab "~price 1 chaos"; position: left 8, top 7)
            new("@From ", false),
            new("<", false, breakOnFail: false),
            new("> ", false, breakOnFail: false),
            new(": Hi, ", true, (e, v) => e.Player = v, typeof(string)),
            new("I would", false, (e, v) => { }, null, "I'd"),
            new(" like to buy your ", false),
            new(" listed for ", true, (e, v) => e.Item = new Item(v), typeof(string), " for my "),
            new(" in ", true, (e, v) => e.Price = new Price(v), typeof(double)),
            new(" (stash tab \"", true, (e, v) => e.League = v, typeof(string)),
            new("\"; position: left ", true, (e, v) => e.Location.StashTab = v, typeof(string)),
            new(", top ", true, (e, v) => e.Location.Left = int.TryParse(v, out var left) ? left : 0, typeof(string)),
            new(")", true, (e, v) => e.Location.Top = int.TryParse(v, out var top) ? top : 0, typeof(string))
        }
    )
    {
    }

    #endregion
}