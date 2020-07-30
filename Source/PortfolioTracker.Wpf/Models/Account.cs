namespace PortfolioTracker.Wpf.Models
{
    public class Account : ModelBase
    {
        private string  _FirstName;
        public string FirstName
        {
            get { return _FirstName; }
            set 
            {
                _FirstName = value;
                OnPropertyChanged();
            }
        }

        private string _LastName;
        public string LastName
        {
            get { return _LastName; }
            set
            {
                _LastName = value;
                OnPropertyChanged();
            }
        }

        private decimal _Balance;
        public decimal Balance
        {
            get { return _Balance; }
            set
            {
                _Balance = value;
                OnPropertyChanged();
            }
        }

    }
}
