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
    public Task<Bitmap?> CurrencyImage { get; set; } = null!;

    public Price()
    {
    }

    public Price(string text)
    {
        var parts = text.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) return;

        Value = double.TryParse(parts[0], out var price) ? price : 1;
        Currency = string.Join(" ", parts.Skip(1));
    }

    public override string ToString()
    {
        return $"{Value} {Currency}";
    }
}