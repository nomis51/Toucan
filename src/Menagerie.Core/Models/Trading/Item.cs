using Newtonsoft.Json;

namespace Menagerie.Core.Models.Trading;

public class Item
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("quantity")]
    public int Quantity { get; set; } = 1;

    public override string ToString()
    {
        return $"{Quantity} {Name}";
    }
}