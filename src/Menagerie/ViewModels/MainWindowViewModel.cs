using Menagerie.Core.Services;

namespace Menagerie.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Events

    public delegate void OverlayVisibilityChangedEvent(bool isVisible);

    public event OverlayVisibilityChangedEvent? OnOverlayVisibilityChangedEvent;

    #endregion

    #region Constructors

    public MainWindowViewModel()
    {
        AppService.OnOverlayVisibilityChangedEvent += AppService_OnOverlayVisibilityChangedEvent;
    }

    #endregion

    #region Private methods

    private void AppService_OnOverlayVisibilityChangedEvent(bool isVisible)
    {
        OnOverlayVisibilityChangedEvent?.Invoke(isVisible);
    }

    #endregion
}