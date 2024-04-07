using Menagerie.Core.Models.Trading;

namespace Menagerie.Core.Services.Abstractions;

public interface IGameChatService
{
    Task<bool> SendBusyWhisper(Trade trade);
    Task<bool> SendSoldWhisper(Trade trade);
    Task<bool> SendStillInterestedWhisper(Trade trade);
    Task<bool> SendInviteWhisper(Trade trade);
    Task<bool> SendThanksWhisper(Trade trade);
    bool SendInvite(Trade trade);
    bool SendKick(Trade trade);
    bool SendReInvite(Trade trade);
    bool SendTradeRequest(Trade trade);
    bool SendHideoutCommand(Trade? trade = null);
    bool PrepareToSendWhisper(Trade trade);
}