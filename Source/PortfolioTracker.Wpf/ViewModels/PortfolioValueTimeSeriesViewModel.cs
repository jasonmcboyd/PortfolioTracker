using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using PortfolioTracker.EF;
using PortfolioTracker.MarketProvider;
using PortfolioTracker.Wpf.Commands;
using PortfolioTracker.Wpf.DataStructures;
using PortfolioTracker.Wpf.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioTracker.Wpf.ViewModels
{
    public class PortfolioValueTimeSeriesViewModel : ViewModelBase
    {
        public PortfolioValueTimeSeriesViewModel(
            PortfolioTrackerContext dbContext,
            IMarketProvider provider)
        {
            DbContext = dbContext;
            Provider = provider;

            LoadPortfolioValueTimeSeriesDataCommand = new DelegateCommand<int>(LoadPortfolioValueTimeSeriesData, CanLoadPortfolioValueTimeSeriesData);

            LoadPortfolioValueTimeSeriesDataCommand.Execute(30);

            XAxisFormatter = value =>
            {
                var days = (DateTime.Today - MinDate).Days;
                var date = new DateTime((long)(value * TimeSpan.FromHours(1).Ticks));

                if (days < 365)
                {
                    return value < 0 ? "" : date.ToString("MMM dd");
                }
                else
                {
                    return value < 0 ? "" : date.ToString("MMM `yy");
                }
            };
        }

        private int _AccountId;
        public int AccountId
        {
            get { return _AccountId; }
            set
            {
                if (_AccountId != value)
                {
                    _AccountId = value;
                    OnPropertyChanged();
                    LoadAccountAsync(_AccountId);
                }
            }
        }

        public PortfolioTrackerContext DbContext { get; }
        public IMarketProvider Provider { get; }

        private SeriesCollection _AccountValueSeries;
        public SeriesCollection AccountValueSeries
        {
            get { return _AccountValueSeries; }
            private set
            {
                _AccountValueSeries = value;
                OnPropertyChanged();
            }
        }

        private List<Transaction> Transactions { get; set; }

        private async Task LoadAccountAsync(int accountId)
        {
            IncrementBusyCounter();

            try
            {
                await LoadTransactionDataAsync(accountId);
                var days = (DateTime.Today - Transactions[0].TransactionDate.Date).Days;
                await LoadAccountDataAsync(days);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            finally
            {
                DecrementBusyCounter();
            }
        }

        private async Task LoadTransactionDataAsync(int accountId)
        {
            Transactions = await
                    DbContext
                    .Transactions
                    .Where(x => x.AccountId == accountId)
                    .OrderBy(x => x.TransactionDate)
                    .ToListAsync();
        }

        private async Task LoadAccountDataAsync(int days)
        {
            LoadPortfolioValueTimeSeriesDataCommand.CanExecuteFlag = false;

            try
            {
                var historicalDataTasks = await
                    Transactions
                    .Select(x => x.TickerSymbol)
                    .Distinct()
                    .Select(x => Provider.GetHistoricalPriceQuotes(x, DateTime.Today - TimeSpan.FromDays(days)))
                    .WhenAll();

                var positions =
                    historicalDataTasks
                    .Select(x => x.Result)
                    .Select(x => new PositionValueTimeSeries(x.TickerSymbol, Transactions, x.HistoricalPriceQuotes))
                    .ToList();

                var portfolio = new PortfolioValueTimeSeries(positions);

                CurrentPortfolioValue = portfolio.Last().Value;

                var historicalQuoteConfig =
                    Mappers
                    .Xy<DateValue>()
                    .X(quote => (double)quote.Date.Ticks / TimeSpan.FromHours(1).Ticks)
                    .Y(quote => (double)quote.Value);

                AccountValueSeries = new SeriesCollection(historicalQuoteConfig)
                    {
                        new LineSeries
                        {
                            Values = new ChartValues<DateValue>(portfolio)
                        }
                    };
                ErrorMessage = null;
            }
            finally
            {
                LoadPortfolioValueTimeSeriesDataCommand.CanExecuteFlag = true;
            }
        }

        private decimal _CurrentPortfolioValue;
        public decimal CurrentPortfolioValue
        {
            get { return _CurrentPortfolioValue; }
            private set
            {
                _CurrentPortfolioValue = value;
                OnPropertyChanged();
            }
        }

        private DateTime _MinDate;
        public DateTime MinDate
        {
            get { return _MinDate; }
            set
            {
                if (_MinDate != value)
                {
                    _MinDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ErrorMessage;
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                if (_ErrorMessage != value)
                {
                    _ErrorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private void LoadPortfolioValueTimeSeriesData(int days) => MinDate = DateTime.Today - TimeSpan.FromDays(days);

        private bool CanLoadPortfolioValueTimeSeriesData(int days)
        {
            return days > 0;
        }

        public DelegateCommand<int> LoadPortfolioValueTimeSeriesDataCommand { get; }

        public Func<double, string> XAxisFormatter { get; }
        public Func<double, string> YAxisFormatter { get; } = value => $"${value / 1_000}K";
        public override void Dispose()
        {
            // Nothing to dispose.
        }
    }
}
