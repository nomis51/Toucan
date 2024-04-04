using Menagerie.Core.Services;
using Serilog;

namespace Menagerie.Core.Helpers;

public class LogsHelper
{
    #region Public methods

    public static void Initialize()
    {
        var logsFolder = AppService.Instance.GetLogsFolder();
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(Path.Join(logsFolder, ".txt"), rollingInterval: RollingInterval.Day)
            .WriteTo.Debug()
            .CreateLogger();
    }

    #endregion
}