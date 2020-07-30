namespace PortfolioTracker.Wpf.Models
{
    public class Position : ModelBase
    {
        private string _TickerSymbol;
        public string TickerSymbol
        {
            get { return _TickerSymbol; }
            set
            {
                if (_TickerSymbol != value)
                {
                    _TickerSymbol = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _Quantity;
        public int Quantity
        {
            get { return _Quantity; }
            set
            {
                if (_Quantity != value)
                {
                    _Quantity = value;
                    OnPropertyChanged();
                    UpdateDayChange();
                    UpdateMarketValue();
                }
            }
        }

        private decimal _CostBasis;
        public decimal CostBasis
        {
            get { return _CostBasis; }
            set
            {
                if (_CostBasis != value)
                {
                    _CostBasis = value;
                    OnPropertyChanged();
                    UpdateGain();
                }
            }
        }

        private decimal? _Open;
        public decimal? Open
        {
            get { return _Open; }
            set
            {
                if (_Open != value)
                {
                    _Open = value;
                    OnPropertyChanged();
                    UpdatePriceChange();
                }
            }
        }

        private decimal? _Close;
        public decimal? Close
        {
            get { return _Close; }
            set
            {
                if (_Close != value)
                {
                    _Close = value;
                    OnPropertyChanged();
                }
            }
        }

        private decimal _Price;
        public decimal Price
        {
            get { return _Price; }
            set
            {
                if (_Price != value)
                {
                    _Price = value;
                    OnPropertyChanged();
                    UpdatePriceChange();
                    UpdateMarketValue();
                }
            }
        }

        private void UpdatePriceChange() => PriceChange = Price - Open;
        private decimal? _PriceChange;
        public decimal? PriceChange
        {
            get { return _PriceChange; }
            private set
            {
                if (_PriceChange != value)
                {
                    _PriceChange = value;
                    OnPropertyChanged();
                    UpdateDayChange();
                }
            }
        }

        private void UpdateDayChange() => DayChange = PriceChange * Quantity;
        private decimal? _DayChange;
        public decimal? DayChange
        {
            get { return _DayChange; }
            private set
            {
                if (_DayChange != value)
                {
                    _DayChange = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateMarketValue() => MarketValue = Price * Quantity;
        private decimal _MarketValue;
        public decimal MarketValue
        {
            get { return _MarketValue; }
            private set
            {
                if (_MarketValue != value)
                {
                    _MarketValue = value;
                    OnPropertyChanged();
                    UpdateGain();
                }
            }
        }

        private void UpdateGain() => Gain = MarketValue - CostBasis;
        private decimal _Gain;
        public decimal Gain 
        { 
            get { return _Gain; }
            set
            {
                if (_Gain != value)
                {
                    _Gain = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
