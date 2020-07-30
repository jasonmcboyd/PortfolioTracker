using System.Windows;
using System.Windows.Controls.Primitives;

namespace PortfolioTracker.Wpf.Behaviors
{
    public static class CommandBindingHelper
    {
        public static DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(CommandBindingHelper),
            new PropertyMetadata(CommandParameter_Changed));

        private static void CommandParameter_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ButtonBase target)
            {
                target.CommandParameter = e.NewValue;
                var temp = target.Command;
                // Have to set it to null first or CanExecute won't be called.
                target.Command = null;
                target.Command = temp;
            }
        }

        public static object GetCommandParameter(ButtonBase target) => target.GetValue(CommandParameterProperty);
        
        public static void SetCommandParameter(ButtonBase target, object value) => target.SetValue(CommandParameterProperty, value);
    }
}
