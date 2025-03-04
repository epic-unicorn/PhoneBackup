using MediaDevices;
using PhoneBackup.Commands;
using PhoneBackup.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace PhoneBackup.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly ICommand m_RefreshDevicesCommand;
        public ICommand RefreshDevicesCommand
        {
            get { return m_RefreshDevicesCommand; }
        }

        private readonly ICommand m_AddFolderCommand;
        public ICommand AddFolderCommand
        {
            get { return m_AddFolderCommand; }
        }

        private Phone m_SelectedPhone;
        public Phone SelectedPhone
        {
            get { return m_SelectedPhone; }
            set
            {
                m_SelectedPhone = value;
                OnPropertyChanged(nameof(SelectedPhone));
            }
        }

        private string m_SelectedPhoneDirectory;
        public string SelectedPhoneDirectory
        {
            get { return m_SelectedPhoneDirectory; }
            set
            {
                m_SelectedPhoneDirectory = value;
                OnPropertyChanged(nameof(SelectedPhoneDirectory));
            }
        }

        private ObservableCollection<Phone> m_Phones;
        public ObservableCollection<Phone> Phones
        {
            get { return m_Phones; }
            set
            {
                m_Phones = value;
                OnPropertyChanged(nameof(Phones));
            }
        }

        private ObservableCollection<string> m_PhoneDirectories;
        public ObservableCollection<string> PhoneDirectories
        {
            get { return m_PhoneDirectories; }
            set
            {
                m_PhoneDirectories = value;
                OnPropertyChanged(nameof(PhoneDirectories));
            }
        }

        private ObservableCollection<BackupEntry> m_BackupEntries;
        public ObservableCollection<BackupEntry> BackupEntries
        {
            get { return m_BackupEntries; }
            set
            {
                m_BackupEntries = value;
                OnPropertyChanged(nameof(BackupEntries));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowViewModel()
        {
            m_RefreshDevicesCommand = new RefreshDevicesCommand(this);
            m_AddFolderCommand = new AddFolderCommand(this);

            m_Phones = new ObservableCollection<Phone>();
            m_PhoneDirectories = new ObservableCollection<string>();
            m_BackupEntries = new ObservableCollection<BackupEntry>();
        }

        internal void HandleRefreshDevicesCommand(object parameter)
        {
            Phones.Clear();

            var devices = MediaDevice.GetDevices();
            foreach (var device in devices)
            {
                device.Connect();

                Phones.Add(new Phone()
                {
                    Name = device.Description,
                    Vendor = device.Manufacturer,
                    DeviceId = device.DeviceId
                });

                SelectedPhone = Phones[0];
                ReadFolders(device);

                device.Disconnect();
            }
        }

        private void ReadFolders(MediaDevice device)
        {
            PhoneDirectories.Clear();
            var directories = device.EnumerateDirectories(device.GetRootDirectory().FullName);
            foreach (var directory in directories)
            {
                var subDirectories = device.EnumerateDirectories(System.IO.Path.GetFileName(directory));
                foreach (var subDirectory in subDirectories)
                {
                    PhoneDirectories.Add(Path.GetFileName(subDirectory) );
                }
            }
        }

        internal void AddFolder(object parameter)
        {
            var backupDirectory = parameter as string;
            if (backupDirectory != null)
            {
                //todo check exist

                BackupEntries.Add(new BackupEntry() { Phone = SelectedPhone, SourceDirectory = backupDirectory, DestinationDirectory = "TODO" });
            }
        }
    }
}
