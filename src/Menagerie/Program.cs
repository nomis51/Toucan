using Avalonia;
using Avalonia.ReactiveUI;
using System;
using Menagerie.Core;
using Menagerie.Core.Helpers;
using Menagerie.Core.Services;
using Serilog;

namespace Menagerie;

sealed class Program
{
    #region Public methods

    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            AppService.Instance.Initialize(new ServicesDependencies());
            UpdateHelper.HookSquirrel();
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Error("Unhandled exception: {Message} {StackTrace}", e.Message, e.StackTrace);
        }
    }

    #endregion

    #region Private methods

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    }

    #endregion
}