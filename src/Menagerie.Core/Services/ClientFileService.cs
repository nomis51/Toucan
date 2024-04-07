using System.Text.RegularExpressions;
using Menagerie.Core.Services.Abstractions;
using Serilog;

namespace Menagerie.Core.Services;

public class ClientFileService : IClientFileService
{
    #region Constants

    private const int FileCheckInterval = 500;
    private const int WaitOnErrorInterval = 5000;

    #endregion

    #region Members

    private long _endOfFilePosition;

    #endregion

    #region Public methods

    public void ClientFileLocationFound()
    {
        FindEndOfFile();
        WatchFile();
    }

    #endregion

    #region Private methods

    private void WatchFile()
    {
        var thread = new Thread(() =>
        {
            var newLines = new List<string>();
            while (true)
            {
                newLines.Clear();

                try
                {
                    do
                    {
                        Thread.Sleep(FileCheckInterval);

                        newLines = ReadNewLines();
                    } while (!newLines.Any());

                    foreach (var line in newLines)
                    {
                        AppService.Instance.ClientFileLineFound(line);
                    }
                }
                catch (Exception e)
                {
                    Log.Warning("Error while reading client file: {Message}", e.Message);
                    Thread.Sleep(WaitOnErrorInterval);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        })
        {
            IsBackground = true
        };
        thread.Start();
    }

    private void FindEndOfFile()
    {
        var file = File.Open(
            AppService.Instance.GetClientFilePath(),
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite
        );
        _endOfFilePosition = file.Length - 1;
        file.Close();
    }

    private List<string> ReadNewLines()
    {
        var lines = new List<string>();

        var currentPosition = _endOfFilePosition;

        FindEndOfFile();

        if (currentPosition >= _endOfFilePosition)
        {
            return lines;
        }

        var file = File.Open(
            AppService.Instance.GetClientFilePath(),
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite
        );
        file.Position = currentPosition;
        var reader = new StreamReader(file);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (!string.IsNullOrEmpty(line))
            {
                lines.Add(RemoveSpecialChars(line));
            }
        }

        reader.Close();
        file.Close();

        return lines;
    }

    private static string RemoveSpecialChars(string str)
    {
        return string.IsNullOrEmpty(str) ? str : Regex.Replace(str, @"[\r\n]", string.Empty);
    }

    #endregion
}