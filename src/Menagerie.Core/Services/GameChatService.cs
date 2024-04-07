using System.Text;
using Menagerie.Core.Helpers;
using Menagerie.Core.Models.Trading;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class GameChatService : IGameChatService
{
    #region Constants

    public const string ItemVariable = "item";
    public const string PriceVariable = "price";
    public const string PlayerVariable = "player";
    public const string LocationVariable = "location";

    private Dictionary<string, Func<object, string>> _variables = new()
    {
        ["item"] = e => ((Trade)e).Item.ToString(),
        ["price"] = e => ((Trade)e).Price.ToString(),
        ["player"] = e => ((Trade)e).Player,
        ["location"] = e => "", // TODO: get location
    };

    #endregion

    #region Public methods

    public async Task<bool> SendBusyWhisper(Trade trade)
    {
        var settings = await AppService.Instance.GetSettings();
        if (string.IsNullOrEmpty(settings.Whispers.Busy)) return true;

        return SendWhisper(
            trade.Player,
            Render(
                settings.Whispers.Busy,
                trade,
                new[]
                {
                    ItemVariable,
                    PriceVariable,
                    PlayerVariable
                }
            )
        );
    }

    public async Task<bool> SendSoldWhisper(Trade trade)
    {
        var settings = await AppService.Instance.GetSettings();
        if (string.IsNullOrEmpty(settings.Whispers.Sold)) return true;

        return SendWhisper(
            trade.Player,
            Render(
                settings.Whispers.Sold,
                trade,
                new[]
                {
                    ItemVariable,
                    PriceVariable,
                    PlayerVariable
                }
            )
        );
    }

    public async Task<bool> SendStillInterestedWhisper(Trade trade)
    {
        var settings = await AppService.Instance.GetSettings();
        if (string.IsNullOrEmpty(settings.Whispers.StillInterested)) return true;

        return SendWhisper(
            trade.Player,
            Render(
                settings.Whispers.StillInterested,
                trade,
                new[]
                {
                    ItemVariable,
                    PriceVariable,
                    PlayerVariable
                }
            )
        );
    }

    public async Task<bool> SendInviteWhisper(Trade trade)
    {
        var settings = await AppService.Instance.GetSettings();
        if (string.IsNullOrEmpty(settings.Whispers.Invite)) return true;

        return SendWhisper(
            trade.Player,
            Render(
                settings.Whispers.Invite,
                trade,
                new[]
                {
                    ItemVariable,
                    PriceVariable,
                    PlayerVariable
                }
            )
        );
    }

    public async Task<bool> SendThanksWhisper(Trade trade)
    {
        var settings = await AppService.Instance.GetSettings();
        if (string.IsNullOrEmpty(settings.Whispers.Thanks)) return true;

        return SendWhisper(
            trade.Player,
            Render(
                settings.Whispers.Thanks,
                trade,
                new[]
                {
                    ItemVariable,
                    PriceVariable,
                    PlayerVariable
                }
            )
        );
    }

    public bool SendInvite(Trade trade)
    {
        return Send($"/invite {trade.Player}");
    }

    public bool SendKick(Trade trade)
    {
        return Send($"/kick {trade.Player}");
    }

    public bool SendReInvite(Trade trade)
    {
        return SendKick(trade) && SendInvite(trade);
    }

    public bool SendTradeRequest(Trade trade)
    {
        return Send($"/tradewith {trade.Player}");
    }

    public bool SendHideoutCommand(Trade? trade = null)
    {
        return Send(trade is null ? "/hideout" : $"/hideout {trade.Player}");
    }

    public bool PrepareToSendWhisper(Trade trade)
    {
        return Send($"@{trade.Player} ", false);
    }

    #endregion

    #region Private methods

    private string Render(string content, object input, IEnumerable<string> variables)
    {
        return variables.Where(variable => _variables.ContainsKey(variable))
            .Aggregate(content, (current, variable) =>
                current.Replace("{" + variable + "}", _variables[variable](input))
            );
    }

    private bool SendWhisper(string player, string message)
    {
        return Send($"@{player} {message}");
    }

    private bool Send(string content, bool doSend = true)
    {
        if (!AppService.Instance.FocusGameWindow()) return false;

        ClipboardHelper.SetClipboard(content);
        KeyboardHelper.SendEnter();
        KeyboardHelper.SendControlA();
        KeyboardHelper.SendControlV();

        if (doSend)
        {
            KeyboardHelper.SendEnter();
        }

        ClipboardHelper.ResetClipboardValue();

        return true;
    }

    #endregion
}