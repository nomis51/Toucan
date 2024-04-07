using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing;
using Menagerie.Core.Models.Trading;

namespace Menagerie.Core.Parsers;

public class RussianOutgoingTradeParser : Parser<OutgoingTrade>
{
    #region Constructors

    public RussianOutgoingTradeParser() : base(
        // @To .+: hello, want buy and you .+ behind .+ in league .+
        new Regex("@.+ Здравствуйте, хочу купить у вас .+ за [0-9\\.]+ .+ в лиге .+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token<OutgoingTrade>>
        {
            new("@", false),
            new(" Здравствуйте, ", true, (e, v) => e.Player = v, typeof(string)),
            new("хочу купить у вас ", false, (e, v) => { }),
            new(" за ", true, (e, v) => e.Item = new Item(v), typeof(string)),
            new(" в лиге ", true, (e, v) => e.Price = new Price(v), typeof(double)),
            new(".", true, (e, v) => e.League = v, typeof(string), endOfString: true)
        }
    )
    {
    }

    #endregion
}