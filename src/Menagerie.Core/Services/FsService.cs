using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class FsService : IFsService
{
    #region Props

    public string AppFolder => Path.GetFullPath(
        Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".menagerie"
        )
    );

    public string LogsFolder => Path.Combine(AppFolder, "logs");
    public string GameFolder { get; private set; } = string.Empty;
    public string ClientFilePath => Path.Join(GameFolder, "logs", "Client.txt");

    #endregion

    #region Public methods

    public void GameProcessFound(string path)
    {
        GameFolder = path;
    }

    #endregion
}