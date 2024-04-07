using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing;
using Menagerie.Core.Models.Trading;

namespace Menagerie.Core.Parsers;

public class KoreanOutgoingTradeParser : Parser<OutgoingTrade>
{
    #region Constructors

    // Need to take korean whisper right from the clipboard instead of client.txt
    public KoreanOutgoingTradeParser() : base(
        // item = fevered mind cobalt jewel 5 chaos
        // @PLAYER hello,    LEAGUE league excited  mind cobalt jewel_     PRICE_      buy     I would like to
        // @bro 안녕하세요, 파수꾼  리그의  흥분한    마음 코발트색 주얼(을)를 5 chaos(으)로 구매하고 싶습니다
        new Regex("@.+ 안녕하세요, .+ 리그의 .+\\(을\\)를 [0-9\\.]+ .+\\(으\\)로 구매하고 싶습니다",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token<OutgoingTrade>>
        {
            new("@", false),
            new(" 안녕하세요, ", true, (e, v) => e.Player = v, typeof(string)),
            new(" 리그의 ", true, (e, v) => e.League = v, typeof(string)),
            new("(을)를 ", true, (e, v) => e.Item = new Item(v), typeof(string)),
            new("(으)로 구매하고 싶습니다", true, (e, v) => e.Price = new Price(v), typeof(string))
        }
    )
    {
    }

    #endregion
}