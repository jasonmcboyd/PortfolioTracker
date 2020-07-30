using PortfolioTracker.EF;
using PortfolioTracker.Wpf.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioTracker.Wpf.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
        public HistoryViewModel(PortfolioTrackerContext dbContext)
        {
            DbContext = dbContext;
        }

        private async Task LoadAccountDataAsync(int accountId)
        {
            IncrementBusyCounter();

            try
            {
                var transactionsTask =
                    DbContext
                    .Transactions
                    .Where(x => x.AccountId == accountId)
                    .ToListAsync();

                var transfersTask =
                    DbContext
                    .Transfers
                    .Where(x => x.AccountId == accountId)
                    .ToListAsync();

                await Task.WhenAll(transactionsTask, transfersTask);

                transactionsTask
                .Result
                .Select(x => new HistoricalAction
                {
                    TickerSymbol = x.TickerSymbol,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Amount = x.Quantity * x.Price * (x.TransactionType.ToLower() == "b" ? -1 : 1),
                    Action = x.TransactionType.ToLower() == "b" ? "Buy" : "Sell",
                    Date = x.TransactionDate
                })
                .Concat(
                    transfersTask
                    .Result
                    .Select(x => new HistoricalAction
                    {
                        Amount = x.Amount,
                        Action = x.TransferType.ToLower() == "d" ? "Deposit" : "Withdrawal",
                        Date = x.TransferDate
                    }))
                .OrderByDescending(x => x.Date)
                .ForEach(HistoricalActions.Add);
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

        protected override void OnAccountIdChanged(int accountId)
        {
            LoadAccountDataAsync(accountId);

            base.OnAccountIdChanged(accountId);
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

        private PortfolioTrackerContext DbContext { get; }

        public ObservableCollection<HistoricalAction> HistoricalActions { get; } = new ObservableCollection<HistoricalAction>();

        public override void Dispose()
        {
            // Nothing to do.
        }
    }
}
