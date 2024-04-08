using System.Drawing;
using Newtonsoft.Json;

namespace Menagerie.Core.Models.Trading;

public class Price
{
    [JsonProperty("value")]
    public double Value { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonProperty("currencyImageUrl")]
    public string CurrencyImageUrl { get; set; }

    public Price()
    {
    }

    public Price(string text)
    {
        var parts = text.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        if (parts.Length > 1)
        {
            var ok = int.TryParse(parts[0], out var value);
            Value = ok ? value : 1;
            Currency = ok ? string.Join(" ", parts.Skip(1)) : text;
        }
        else
        {
            Value = 1;
            Currency = text;
        }
    }

    public override string ToString()
    {
        return $"{Value} {Currency}";
    }
}