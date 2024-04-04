using Menagerie.Core.Enums;

namespace Menagerie.Core.Models.Trading;

public class OutgoingTrade : Trade
{
    public OutgoingTrade()
    {
        Type = TradeType.Outgoing;
    }
}