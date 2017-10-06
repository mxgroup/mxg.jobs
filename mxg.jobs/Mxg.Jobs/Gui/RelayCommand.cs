using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Mxg.Jobs.Gui
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Создание новой команды, всегда разрешённой к исполнению
        /// </summary>
        /// <param name="execute">Логика выполнения.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Создание новой команды
        /// </summary>
        /// <param name="execute">Логика выполнения.</param>
        /// <param name="canExecute">Разрешение выполнения.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
