using System.Collections.ObjectModel;

namespace Menagerie.ViewModels;

public class IncomingTradesWindowViewModel : ViewModelBase
{
    #region Members

    public ObservableCollection<IncomingTradeViewModel> Trades { get; set; } = new();

    #endregion
}