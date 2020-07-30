using PortfolioTracker.Wpf.ViewModels;
using SimpleInjector;

namespace PortfolioTracker.Wpf.DependencyInjection
{
    public class ViewModelFactory
    {
        public ViewModelFactory(Container container)
        {
            Container = container;
        }

        private Container Container { get; }

        public IViewModel GetInstance<T>() where T : class, IViewModel => Container.GetInstance<T>();
    }
}
