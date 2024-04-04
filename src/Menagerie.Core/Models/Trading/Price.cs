using Newtonsoft.Json;

namespace Menagerie.Core.Models.Trading;

public class Price
{
    [JsonProperty("value")]
    public double Value { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonProperty("currencyImageUrl")]
    public string CurrencyImageUrl { get; set; } = string.Empty;
}