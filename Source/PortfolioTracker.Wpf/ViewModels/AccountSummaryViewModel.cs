using PortfolioTracker.MarketProvider;
using PortfolioTracker.Wpf.Events;
using PortfolioTracker.Wpf.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PortfolioTracker.Wpf.ViewModels
{
    public class AccountSummaryViewModel : ViewModelBase
    {
        public AccountSummaryViewModel(
            EF.PortfolioTrackerContext dbContext,
            IMarketProvider provider,
            PortfolioValueTimeSeriesViewModel portfolioValueTimeSeriesViewModel)
        {
            DbContext = dbContext;
            Provider = provider;
            PortfolioValueTimeSeriesViewModel = portfolioValueTimeSeriesViewModel;

            PortfolioValueTimeSeriesViewModel.IsBusyChanged += ChildIsBusyChangedEventHandler;
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
            PortfolioValueTimeSeriesViewModel.AccountId = accountId;
            LoadAccountSummaryAsync(accountId);
            base.OnAccountIdChanged(accountId);
        }

        private async Task LoadPositionsAsync(int accountId)
        {
            try
            {
                var openPositions = await
                    DbContext
                    .Positions
                    .Where(x => x.AccountId == accountId && x.Quantity > 0)
                    .Select(x => new Position
                    {
                        TickerSymbol = x.TickerSymbol,
                        Quantity = x.Quantity,
                        CostBasis = x.CostBasis
                    })
                    .ToListAsync();
                
                var openCloseQuotes = await
                    Provider
                    .GetPricesAsync(openPositions.Select(x => x.TickerSymbol).ToArray());

                openPositions
                .Join(
                    openCloseQuotes,
                    inner => inner.TickerSymbol,
                    outer => outer.TickerSymbol,
                    (inner, outer) => new { Position = inner, outer.Open, outer.Close })
                .ForEach(x =>
                {
                    x.Position.Open = x.Open;
                    x.Position.Close = x.Close;
                    Positions.Add(x.Position);
                });

                LoadPositionsErrorMessage = null;
            }
            catch (Exception e)
            {
                LoadPositionsErrorMessage = e.Message;
            }
        }

        private async Task LoadAccountAsync(int accountId)
        {
            try
            {
                var account = await
                    DbContext
                    .Accounts
                    .Where(x => x.Id == accountId)
                    .Select(x => new Account
                    {
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Balance = x.Balance
                    })
                    .FirstAsync();

                Account = account;
            }
            catch (Exception e)
            {
                LoadAccountErrorMessage = e.Message;
            }
        }

        private async Task LoadAccountSummaryAsync(int accountId)
        {
            IncrementBusyCounter();

            try
            {
                var loadPositionsTask = LoadPositionsAsync(accountId);
                var loadAccountTask = LoadAccountAsync(accountId);

                await Task.WhenAll(loadPositionsTask, loadAccountTask);

                // Update the prices every 15 seconds. 15 seconds should be sufficient
                // to keep requests per hour under 500 with the app running full time.
                PriceQuotesSubscription =
                    Provider
                    .GetPricesObservable(10, Positions.Select(x => x.TickerSymbol).ToArray())
                    .ObserveOnDispatcher()
                    .Subscribe(x =>
                    {
                        for (int i = 0; i < x.Count; i++)
                        {
                            Positions[i].Price = x[i].Price;
                        }
                    });
            }
            finally
            {
                DecrementBusyCounter();
            }
        }

        private EF.PortfolioTrackerContext DbContext { get; }
        public IMarketProvider Provider { get; }

        private IDisposable PriceQuotesSubscription { get; set; }

        public ObservableCollection<Position> Positions { get; } = new ObservableCollection<Position>();

        private string _LoadPositionsErrorMessage;
        public string LoadPositionsErrorMessage
        {
            get { return _LoadPositionsErrorMessage; }
            private set
            {
                _LoadPositionsErrorMessage = value;
                OnPropertyChanged();
            }
        }

        private string _LoadAccountErrorMessage;
        public string LoadAccountErrorMessage
        {
            get { return _LoadAccountErrorMessage; }
            private set
            {
                _LoadAccountErrorMessage = value;
                OnPropertyChanged();
            }
        }

        private Account _Account;
        public Account Account
        {
            get { return _Account; }
            set
            {
                _Account = value;
                OnPropertyChanged();
            }
        }

        public PortfolioValueTimeSeriesViewModel PortfolioValueTimeSeriesViewModel { get; }
        
        public override void Dispose()
        {
            PriceQuotesSubscription?.Dispose();
            if (PortfolioValueTimeSeriesViewModel != null)
            {
                PortfolioValueTimeSeriesViewModel.IsBusyChanged -= ChildIsBusyChangedEventHandler;
            }
        }
    }
}
