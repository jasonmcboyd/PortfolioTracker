using PortfolioTracker.EF;
using PortfolioTracker.MarketProvider.Tiingo;
using PortfolioTracker.Wpf.ViewModels;
using System;
using System.Configuration;
using System.Net.Http;
using System.Windows;

namespace PortfolioTracker.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var apiToken = new TiingoApiToken(ConfigurationManager.AppSettings["tiingoApiToken"]);

            if (apiToken == null)
            {
                throw new ConfigurationErrorsException("Missing required application settings: tiingoApiToken");
            }

            InitializeComponent();
        }
    }
}
