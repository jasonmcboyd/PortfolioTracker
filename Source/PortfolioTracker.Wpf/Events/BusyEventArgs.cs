using System;

namespace PortfolioTracker.Wpf.Events
{
    public class BusyEventArgs : EventArgs
    {
        public BusyEventArgs(bool isBusy)
        {
            IsBusy = isBusy;
        }

        public bool IsBusy { get; }
    }
}
