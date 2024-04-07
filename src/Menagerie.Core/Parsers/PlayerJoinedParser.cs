using System.Text.RegularExpressions;
using Menagerie.Core.Models.Events;
using Menagerie.Core.Models.Parsing;

namespace Menagerie.Core.Parsers;

public class PlayerJoinedParser : Parser<PlayerJoinedEvent>
{
    public PlayerJoinedParser() : base(
        new Regex(" has joined the area", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token<PlayerJoinedEvent>>
        {
            new("] : ", false),
            new(" has joined the area.", true, (e, v) => e.Player = v, typeof(string))
        }
    )
    {
    }
}