using Menagerie.Core.Enums;

namespace Menagerie.Core.Models.Trading;

public class IncomingTrade : Trade
{
    public IncomingTrade()
    {
        Type = TradeType.Incoming;
    }
}