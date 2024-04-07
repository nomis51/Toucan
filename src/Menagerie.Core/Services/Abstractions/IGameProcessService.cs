namespace Menagerie.Core.Services.Abstractions;

public interface IGameProcessService
{
    string ProcessLocation { get; }
    int ProcessId { get; }

    bool FocusGameWindow();
}