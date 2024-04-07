using Menagerie.Core.Enums;
using Newtonsoft.Json;

namespace Menagerie.Core.Models.Trading;

public abstract class Trade
{
    [JsonProperty("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonProperty("type")]
    public TradeType Type { get; set; }

    [JsonProperty("time")]
    public DateTime Time { get; set; } = DateTime.Now;

    [JsonProperty("player")]
    public string Player { get; set; } = string.Empty;

    [JsonProperty("item")]
    public Item Item { get; set; } = new();

    [JsonProperty("price")]
    public Price Price { get; set; } = new();

    [JsonProperty("location")]
    public Location Location { get; set; } = new();

    [JsonProperty("league")]
    public string League { get; set; } = string.Empty;

    [JsonProperty("whisper")]
    public string Whisper { get; set; } = string.Empty;

    [JsonIgnore]
    public TradeState State { get; set; } = TradeState.Initial;
}