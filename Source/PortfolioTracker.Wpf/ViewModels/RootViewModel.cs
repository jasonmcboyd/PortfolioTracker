using PortfolioTracker.EF;
using PortfolioTracker.Wpf.Commands;
using PortfolioTracker.Wpf.DependencyInjection;
using PortfolioTracker.Wpf.Events;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace PortfolioTracker.Wpf.ViewModels
{
    public class RootViewModel : ViewModelBase
    {
        public RootViewModel(
            ViewModelFactory viewModelFactory,
            PortfolioTrackerContext portfolioTrackerContext)
        {
            ViewModelFactory = viewModelFactory;
            PortfolioTrackerContext = portfolioTrackerContext;
            ChangeViewModelCommand = new DelegateCommand<string>(x =>
            {
                if (ChildViewModel != null)
                {
                    ChildViewModel.IsBusyChanged -= ChildIsBusyChangedEventHandler;
                    if (ChildViewModel.IsBusy)
                    {
                        DecrementBusyCounter();
                    }
                }
                switch (x)
                {
                    case nameof(AccountSummaryViewModel):
                        ChildViewModel = ViewModelFactory.GetInstance<AccountSummaryViewModel>();
                        break;
                    case nameof(HistoryViewModel):
                        ChildViewModel = ViewModelFactory.GetInstance<HistoryViewModel>();
                        break;
                    default:
                        throw new InvalidOperationException("Unknown view model.");
                }
                if (ChildViewModel != null)
                {
                    if (ChildViewModel.IsBusy)
                    {
                        IncrementBusyCounter();
                    }
                    ChildViewModel.IsBusyChanged += ChildIsBusyChangedEventHandler;
                }
                ChildViewModel.AccountId = AccountId;
            });
            //Test();
            ChangeViewModelCommand.Execute(nameof(AccountSummaryViewModel));
        }

        private async Task Test()
        {
            IncrementBusyCounter();
            await Task.Delay(5000);
            DecrementBusyCounter();
        }

        private void ChildIsBusyChangedEventHandler(object o, BusyEventArgs e)
        {
            if (e.IsBusy)
            {
                IncrementBusyCounter();
            }
            else
            {
                DecrementBusyCounter();
            }
        }

        protected override void OnAccountIdChanged(int accountId)
        {
            LoadAccountAsync(accountId);
            if (ChildViewModel != null)
            {
                ChildViewModel.AccountId = accountId;
            }
            base.OnAccountIdChanged(accountId);
        }

        private ViewModelFactory ViewModelFactory { get; }
        private PortfolioTrackerContext PortfolioTrackerContext { get; }

        private async Task LoadAccountAsync(int accountId)
        {
            IncrementBusyCounter();

            try
            {
                // Makes the user wait longer but prevents the busy indicator from
                // flashing and the app appearing to be jittery.
                var psychologicalDelay = Task.Delay(3000);
                var accountTask =
                    PortfolioTrackerContext
                    .Accounts
                    .FirstAsync(x => x.Id == accountId);

                await Task.WhenAll(psychologicalDelay, accountTask);

                var account = accountTask.Result;

                AccountName = $"{account.FirstName} {account.LastName}";
            }
            finally
            {
                DecrementBusyCounter();
            }
        }

        private string _AccountName;
        public string  AccountName
        {
            get { return _AccountName; }
            set
            {
                if (_AccountName != value)
                {
                    _AccountName = value;
                    OnPropertyChanged();
                }
            }
        }

        private IViewModel _ChildViewModel;
        public IViewModel ChildViewModel
        {
            get { return _ChildViewModel; }
            set
            {
                if (_ChildViewModel != value)
                {
                    _ChildViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        public DelegateCommand<string> ChangeViewModelCommand { get; }

        public override void Dispose()
        {
            var childViewModel = ChildViewModel;
            if (childViewModel != null)
            {
                childViewModel.IsBusyChanged -= ChildIsBusyChangedEventHandler;
            }
        }
    }
}
