using PhoneBackup.ViewModels;
using System.Windows.Input;

namespace PhoneBackup.Commands
{
    public class AddFolderCommand : ICommand
    {
        private readonly MainWindowViewModel m_ViewModel;

        public AddFolderCommand(MainWindowViewModel viewModel)
        {
            m_ViewModel = viewModel;
        }

        public void Execute(object parameter)
        {
            m_ViewModel.AddFolder(parameter);
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
