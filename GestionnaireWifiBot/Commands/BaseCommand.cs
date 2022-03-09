using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GestionnaireWifiBot.Commands
{
    public class BaseCommand : ICommand
    {
        private readonly Action<object> Action;
        private readonly Predicate<object> Predicate;

        public BaseCommand(Action<object> action) : this(action, x => true)
        { }

        public BaseCommand(Action<object> action, Predicate<object> predicate)
        {
            Action = action;
            Predicate = predicate;
        }

        public bool CanExecute(object parameter)
        {
            return Predicate(parameter);
        }
        public void Execute(object parameter)
        {
            Action(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
