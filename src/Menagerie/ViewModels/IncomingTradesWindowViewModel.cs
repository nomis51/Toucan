using System;
using System.Collections.ObjectModel;
using System.Linq;
using Menagerie.Core.Models.Trading;
using Menagerie.Core.Services;
using Menagerie.Enums;
using Menagerie.Services;
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
        var vm = new IncomingTradeViewModel(trade, _tradeSize);
        vm.OnRemoved += RemoveTrade;

        Trades.Add(vm);
        AudioService.Instance.PlayEffect(AudioEffect.NewTrade);

        this.RaisePropertyChanged(nameof(Trades));
    }

    private void RemoveTrade(Guid id)
    {
        var vm = Trades.FirstOrDefault(t => t.Trade.Id == id);
        if (vm is null) return;

        vm.OnRemoved -= RemoveTrade;
        Trades.Remove(vm);
        this.RaisePropertyChanged(nameof(Trades));
    }

    #endregion
}