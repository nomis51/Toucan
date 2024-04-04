namespace Menagerie.Core.Services.Abstractions;

public interface IFsService
{
    string LogsFolder { get; }
    string ClientFilePath { get; }

    void GameProcessFound(string path);
}