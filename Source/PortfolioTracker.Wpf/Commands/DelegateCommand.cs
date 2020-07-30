using System;
using System.Windows.Input;

namespace PortfolioTracker.Wpf.Commands
{
    public class DelegateCommand<T> : ICommand
    {
        public DelegateCommand(Action<T> executeDelegate, bool initialCanExecuteFlag = true) : this(executeDelegate, _ => true, initialCanExecuteFlag)
        {
        }

        public DelegateCommand(
            Action<T> executeDelegate,
            Func<T, bool> canExecuteDelegate,
            bool initialCanExecuteFlag = true)
        {
            ExecuteDelegate = executeDelegate;
            CanExecuteDelegate = canExecuteDelegate;
            CanExecuteFlag = initialCanExecuteFlag;
        }

        private Action<T> ExecuteDelegate { get; }
        private Func<T, bool> CanExecuteDelegate { get; }
        

        public event EventHandler CanExecuteChanged;

        private bool _CanExecuteFlag;
        public bool CanExecuteFlag
        {
            get { return _CanExecuteFlag; }
            set 
            {
                if (_CanExecuteFlag != value)
                {
                    _CanExecuteFlag = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private bool CanExecute(T parameter) => CanExecuteFlag && CanExecuteDelegate(parameter);

        public bool CanExecute(object parameter) => parameter is T typedParameter && CanExecute(typedParameter);

        public void Execute(object parameter)
        {
            if (parameter is T typedParameter && CanExecute(typedParameter))
            {
                ExecuteDelegate(typedParameter);
            }
        }
    }
}
