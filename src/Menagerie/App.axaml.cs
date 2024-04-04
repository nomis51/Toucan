using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Menagerie.Core.Services;
using Menagerie.ViewModels;
using Serilog;
using MainWindow = Menagerie.Windows.MainWindow;

namespace Menagerie;

public class App : Application
{
    #region Constants

    private const string GitHubPageUrl = $"https://github.com/nomis51/{nameof(Menagerie)}";
    private const string GitHubIssuePageUrl = $"{GitHubPageUrl}/issues";

    #endregion

    #region Public methods

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = new AppViewModel();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
                Position = new PixelPoint(-100, -100)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    #endregion

    #region Private methods

    private void MenuItemOpenGitHub_OnClick(object? sender, EventArgs e)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start("explorer.exe", GitHubPageUrl);
            }
            else
            {
                Process.Start(GitHubPageUrl);
            }
        }
        catch (Exception ex)
        {
            Log.Error("Failed to open '{Url}': {Message}", GitHubPageUrl, ex.Message);
        }
    }

    private void MenuItemOpenGitHubIssue_OnClick(object? sender, EventArgs e)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start("explorer.exe", GitHubIssuePageUrl);
            }
            else
            {
                Process.Start(GitHubIssuePageUrl);
            }
        }
        catch (Exception ex)
        {
            Log.Error("Failed to open '{Url}': {Message}", GitHubIssuePageUrl, ex.Message);
        }
    }

    private void MenuItemOpenLogs_OnClick(object? sender, EventArgs e)
    {
        var folder = AppService.Instance.GetLogsFolder().Replace("/", "\\");

        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start("explorer.exe", folder);
            }
        }
        catch (Exception ex)
        {
            Log.Error("Failed to open '{Folder}': {Message}", folder, ex.Message);
        }
    }

    private void MenuItemQuit_OnClick(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }

    private void MenuItemCheckForUpdates_OnClick(object? sender, EventArgs e)
    {
        ((AppViewModel)DataContext!).CheckForUpdates();
    }

    #endregion
}