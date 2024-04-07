using System.Text.RegularExpressions;
using Menagerie.Core.Models.Events;
using Menagerie.Core.Models.Parsing;

namespace Menagerie.Core.Parsers;

public class LocationParser : Parser<LocationChangeEvent>
{
    public LocationParser() : base(
        new Regex(": You have entered .+\\.", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token<LocationChangeEvent>>
        {
            new(": You have entered ", false),
            new(".", true, (e, v) => e.Location = v, typeof(string))
        }
    )
    {
    }
}