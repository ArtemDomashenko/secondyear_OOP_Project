using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Project_WPF.Mvvm
{
    // Simple command for buttons.
    public class RelayCommand : ICommand
    {
        private readonly Action _run;
        private readonly Func<bool> _canRun;
        private readonly Func<Task> _runAsync;

        public RelayCommand(Action run, Func<bool> canRun = null)
        {
            _run = run;
            _canRun = canRun;
        }

        public RelayCommand(Func<Task> runAsync, Func<bool> canRun = null)
        {
            _runAsync = runAsync;
            _canRun = canRun;
        }

        public bool CanExecute(object parameter)
        {
            return _canRun == null || _canRun();
        }

        public async void Execute(object parameter)
        {
            if (_run != null)
            {
                _run();
                return;
            }

            if (_runAsync != null)
                await _runAsync();
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
