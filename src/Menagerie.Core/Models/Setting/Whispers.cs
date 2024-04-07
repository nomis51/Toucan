using Menagerie.Core.Services;
using Newtonsoft.Json;

namespace Menagerie.Core.Models.Setting;

public class Whispers
{
    [JsonProperty("thanks")]
    public string Thanks { get; set; } = "Thanks!";

    [JsonProperty("stillInterested")]
    public string StillInterested { get; set; } = "Are you still interested in my \"{" + GameChatService.ItemVariable + "}\" listed for {" +
                                                  GameChatService.PriceVariable + "}?";

    [JsonProperty("sold")]
    public string Sold { get; set; } = "My \"{" + GameChatService.ItemVariable + "}\" has already been sold";

    [JsonProperty("busy")]
    public string Busy { get; set; } = "I'm busy right now. I'll whisper you for the \"{" + GameChatService.ItemVariable + "}\" when I'm ready";

    [JsonProperty("invite")]
    public string Invite { get; set; } =
        "Your \"{" + GameChatService.ItemVariable + "}\" for {" + GameChatService.PriceVariable + "} is ready to picked up!";
}