using Desktop.Robot;
using Menagerie.Core.Models.Keyboard;
using Winook;

namespace Menagerie.Core.Helpers;

public static class KeyboardHelper
{
    #region Members

    private static KeyboardHook? _hook;
    private static readonly Robot _robot = new();

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

    public static void ClearModifiers()
    {
        _robot.KeyUp(Key.Shift);
        _robot.KeyUp(Key.Control);
        _robot.KeyUp(Key.Alt);
    }

    public static void ClearKey(Key key)
    {
        _robot.KeyUp(key);
    }

    public static void SendControlA()
    {
        ControlledKeyPress(Key.A);
    }

    public static void SendControlV()
    {
        ControlledKeyPress(Key.V);
    }

    public static void SendControlC()
    {
        ControlledKeyPress(Key.C);
    }

    public static void SendControlF()
    {
        ControlledKeyPress(Key.F);
    }

    public static void SendEnter()
    {
        _robot.KeyPress(Key.Enter);
    }

    public static void SendEscape()
    {
        _robot.KeyPress(Key.Esc);
    }

    public static void SendDelete()
    {
        _robot.KeyPress(Key.Delete);
    }

    #endregion

    #region Private methods

    private static void OnMessageReceived(object? sender, KeyboardMessageEventArgs e)
    {
        State.Control = e.Control;
        State.Shift = e.Shift;
        State.Alt = e.Alt;
    }

    private static void ControlledKeyPress(Key key)
    {
        _robot.KeyDown(Key.Control);
        _robot.KeyPress(key);
        _robot.KeyUp(Key.Control);
    }

    #endregion
}