using System.Collections.ObjectModel;
using Menagerie.Core.Models.Trading;
using Menagerie.Core.Services;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class IncomingTradesWindowViewModel : ViewModelBase
{
    #region Members

    public ObservableCollection<IncomingTradeViewModel> Trades { get; set; } = new();

    private int _tradeSize = 80;

    #endregion

    #region Constructors

    public IncomingTradesWindowViewModel()
    {
        AppService.OnNewIncomingTrade += AppService_OnNewIncomingTrade;
    }

    #endregion

    #region Public methods

    public void RemoveAllTrades()
    {
        Trades.Clear();
        this.RaisePropertyChanged(nameof(Trades));
    }
    
    public void SetTradeSize(int size)
    {
        _tradeSize = size;
    }

    #endregion

    #region Private methods

    private void AppService_OnNewIncomingTrade(IncomingTrade trade)
    {
        Trades.Add(new IncomingTradeViewModel(trade, _tradeSize));
        this.RaisePropertyChanged(nameof(Trades));
    }

    #endregion
}