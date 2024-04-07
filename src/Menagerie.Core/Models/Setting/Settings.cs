using Newtonsoft.Json;

namespace Menagerie.Core.Models.Setting;

public class Settings
{
    [JsonProperty("whispers")]
    public Whispers Whispers { get; set; } = new();
}