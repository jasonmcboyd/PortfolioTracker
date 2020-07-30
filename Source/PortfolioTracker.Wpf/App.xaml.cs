using PortfolioTracker.EF;
using PortfolioTracker.MarketProvider;
using PortfolioTracker.MarketProvider.Tiingo;
using PortfolioTracker.Wpf.DependencyInjection;
using PortfolioTracker.Wpf.ViewModels;
using SimpleInjector;
using System.Configuration;
using System.Net.Http;
using System.Windows;

namespace PortfolioTracker.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var container = CreateContainer();
            var viewModel = container.GetInstance<RootViewModel>();
            var window = new MainWindow { DataContext = viewModel };
            window.Show();

            // Hard coding the account id for the purposes of this demo
            // Normally there would be some sort of login process that
            // would establish the user's identity and account id.
            viewModel.AccountId = 1;
        }

        private Container CreateContainer()
        {
            var container = new Container();

            container.RegisterInstance(new TiingoApiToken(ConfigurationManager.AppSettings["tiingoApiToken"]));
            container.RegisterSingleton<IMarketProvider, TiingoMarketProvider>();
            container.RegisterSingleton<HistoryViewModel>();
            container.RegisterSingleton<AccountSummaryViewModel>();
            container.RegisterSingleton<PortfolioValueTimeSeriesViewModel>();
            container.RegisterSingleton<RootViewModel>();
            container.RegisterSingleton<PortfolioTrackerContext>();
            container.RegisterSingleton<ViewModelFactory>();
            container.RegisterSingleton(() => new HttpClient());
            
            return container;
        }
    }
}
