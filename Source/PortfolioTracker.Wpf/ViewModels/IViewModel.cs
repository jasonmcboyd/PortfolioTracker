using PortfolioTracker.Wpf.Events;
using System;
using System.ComponentModel;

namespace PortfolioTracker.Wpf.ViewModels
{
    public interface IViewModel : INotifyPropertyChanged, IDisposable
    {
        event EventHandler<BusyEventArgs> IsBusyChanged;
        bool IsBusy { get; }
        int AccountId { get; set; }
    }
}
