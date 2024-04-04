namespace Menagerie.Core.Enums;

[Flags]
public enum TradeState
{
    Initial = 1,
    Busy = 2,
    StillInterested = 4,
    PlayerInvited = 8,
    PlayerJoined = 16,
    Trading = 32,
    Done = 64
}