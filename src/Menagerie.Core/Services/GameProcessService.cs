using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Menagerie.Core.Services.Abstractions;
using Menagerie.Core.WinApi;
using Serilog;

namespace Menagerie.Core.Services;

public class GameProcessService : IGameProcessService
{
    #region Constants

    private const int ProcessCheckInterval = 5000;

    private readonly string[] _processNames =
    {
        "PathOfExile",
        "PathOfExile_Steam",
        "PathOfExile_x64",
        "PathOfExile_x64Steam",
        "PathOfExileSteam",
    };

    #endregion

    #region Props

    public string ProcessLocation { get; private set; } = string.Empty;
    public int ProcessId { get; private set; }

    #endregion

    #region Public methods

    public Task FindProcess()
    {
        ProcessId = 0;
        ProcessLocation = string.Empty;

        return Task.Run(() =>
        {
            while (true)
            {
                foreach (var processName in _processNames)
                {
                    foreach (var process in Process.GetProcessesByName(processName))
                    {
                        if (process.HasExited) continue;

                        if (!FindProcessLocation(process)) continue;

                        ProcessId = process.Id;
                        WatchProcess();

                        AppService.Instance.GameProcessFound();
                        return;
                    }
                }
            }
        });
    }

    public bool FocusGameWindow()
    {
        if (ProcessId == 0) return false;

        try
        {
            var process = Process.GetProcessById(ProcessId);
            if (process is null || process.HasExited) return false;

            if (IsGameWindowFocused()) return true;

            var noTry = 0;
            while (!(User32.ShowWindow(process.MainWindowHandle, 5) &&
                     User32.SetForegroundWindow(process.MainWindowHandle) &&
                     IsGameWindowFocused()) && noTry < 3)
            {
                ++noTry;
            }

            return noTry < 3;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool IsGameWindowFocused(int delay = 50)
    {
        var process = Process.GetProcessById(ProcessId);
        if (process is null || process.HasExited) return false;

        Thread.Sleep(delay);
        var current = User32.GetForegroundWindow();
        return current == process.MainWindowHandle;
    }

    #endregion

    #region Private methods

    private void WatchProcess()
    {
        var t = new Thread(() =>
        {
            while (true)
            {
                try
                {
                    var process = Process.GetProcessById(ProcessId);
                    if (process.HasExited) break;

                    Thread.Sleep(ProcessCheckInterval);
                }
                catch (Exception e)
                {
                    Log.Information("Game process is not running or has exited: {Message}", e.Message);
                    break;
                }
            }

            _ = FindProcess();
        })
        {
            IsBackground = true
        };
        t.Start();
    }

    private bool FindProcessLocation(Process process)
    {
        try
        {
            if (process.MainModule is null) return false;
            if (string.IsNullOrEmpty(process.MainModule.FileName)) return false;

            var location = Path.GetDirectoryName(process.MainModule.FileName);
            if (string.IsNullOrEmpty(location) || !Directory.Exists(location)) return false;

            ProcessLocation = location;
            return true;
        }
        catch (Win32Exception e)
        {
            Log.Warning("Unable to find 64 bits process location: {Message}\n. Trying 32 bits...", e.Message);

            try
            {
                var size = 1024;
                var filename = new StringBuilder(size);
                var result = Kernel32.QueryFullProcessImageNameW(process.Handle, 0, filename, ref size);

                if (!result || size == 0)
                {
                    Log.Error("Error while reading executable file path. Returned size was {Size}", size);
                    // See https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes
                    var lastError = Kernel32.GetLastError();
                    throw new Exception($"GetLastError() = {lastError}");
                }

                var location = Path.GetDirectoryName(filename.ToString());
                if (string.IsNullOrEmpty(location) || !Directory.Exists(location)) return false;

                ProcessLocation = location;
                return true;
            }
            catch (Win32Exception e2)
            {
                Log.Information("Failed trying to get 32 bits location: {Message}", e2.Message);
            }
            catch (Exception e2)
            {
                Log.Error("Error while getting 32 bits PoE process location: {Message}", e2.Message);
            }
        }
        catch (Exception e)
        {
            Log.Error("Error while getting 64 bits PoE process location: {Message}", e.Message);
        }

        return true;
    }

    #endregion
}