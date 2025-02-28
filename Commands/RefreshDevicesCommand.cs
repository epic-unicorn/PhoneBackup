using PhoneBackup.ViewModels;
using System.Windows.Input;

namespace PhoneBackup.Commands
{
    public class RefreshDevicesCommand : ICommand
    {
        private readonly MainWindowViewModel m_ViewModel;

        public RefreshDevicesCommand(MainWindowViewModel viewModel)
        {
            m_ViewModel = viewModel;
        }

        public void Execute(object parameter)
        {
            m_ViewModel.HandleRefreshDevicesCommand(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}