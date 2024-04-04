using Menagerie.Core.Models.Keyboard;
using Winook;

namespace Menagerie.Core.Helpers;

public class KeyboardHelper
{
    #region Members

    private static KeyboardHook? _hook;

    #endregion

    #region Props

    public static KeyboardState State { get; } = new();

    #endregion

    #region Public methods

    public static Task Initialize(int processId)
    {
        _hook = new KeyboardHook(processId);
        _hook.MessageReceived += OnMessageReceived;
        return _hook.InstallAsync();
    }

    #endregion

    #region Private methods

    private static void OnMessageReceived(object? sender, KeyboardMessageEventArgs e)
    {
        State.Control = e.Control;
        State.Shift = e.Shift;
        State.Alt = e.Alt;
    }

    #endregion
}