using PortfolioTracker.Wpf.Events;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PortfolioTracker.Wpf.ViewModels
{
    public abstract class ViewModelBase : IViewModel
    {
        protected virtual void OnAccountIdChanged(int accountId) { }

        private int _AccountId;
        public int AccountId
        {
            get { return _AccountId; }
            set
            {
                if (_AccountId != value)
                {
                    _AccountId = value;
                    OnAccountIdChanged(_AccountId);
                    OnPropertyChanged();
                }
            }
        }

        public abstract void Dispose();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private object IsBusyLock { get; } = new object();

        private int BusyCounter { get; set; }

        protected void IncrementBusyCounter()
        {
            lock (IsBusyLock)
            {
                BusyCounter++;
                IsBusy = true;
            }
        }

        protected void DecrementBusyCounter()
        {
            lock (IsBusyLock)
            {
                BusyCounter--;
                IsBusy = BusyCounter > 0;
            }
        }

        public event EventHandler<BusyEventArgs> IsBusyChanged;

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            private set
            {
                if (_IsBusy != value)
                {
                    _IsBusy = value;
                    IsBusyChanged?.Invoke(this, new BusyEventArgs(_IsBusy));
                    OnPropertyChanged();
                }
            }
        }
    }
}
