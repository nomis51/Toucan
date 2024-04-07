namespace Menagerie.Core.Services.Abstractions;

public interface IFsService
{
    string LogsFolder { get; }
    string ClientFilePath { get; }
    string AppFolder { get; }

    void GameProcessFound(string path);
}