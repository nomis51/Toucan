using Newtonsoft.Json;

namespace Menagerie.Core.Models.Trading;

public class Location
{
    [JsonProperty("stashTab")]
    public string StashTab { get; set; } = string.Empty;

    [JsonProperty("left")]
    public int Left { get; set; }

    [JsonProperty("top")]
    public int Top { get; set; }
}