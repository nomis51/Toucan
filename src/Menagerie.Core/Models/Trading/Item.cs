using Newtonsoft.Json;

namespace Menagerie.Core.Models.Trading;

public class Item
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("quantity")]
    public int Quantity { get; set; } = 1;

    public Item()
    {
    }

    public Item(string text)
    {
        var parts = text.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        if (parts.Length > 1)
        {
            Quantity = int.TryParse(parts[0], out var quantity) ? quantity : 1;
            Name = string.Join(" ", parts.Skip(1));
        }
        else
        {
            Name = text;
        }
    }

    public override string ToString()
    {
        return $"{Quantity} {Name}";
    }
}