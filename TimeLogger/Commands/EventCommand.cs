using System;
using System.Windows.Input;

namespace TimeLogger.Commands
{
    public class EventCommand : ICommand
    {
        public event EventHandler? Action;

        event EventHandler? ICommand.CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        bool ICommand.CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Action?.Invoke(null, EventArgs.Empty);
        }
    }
}
