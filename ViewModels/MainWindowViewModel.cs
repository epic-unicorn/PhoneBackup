using FolderNavigationDemo;
using MediaDevices;
using Microsoft.VisualStudio.PlatformUI;
using PhoneBackup.Commands;
using PhoneBackup.Models;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PhoneBackup.ViewModels
{
    public class MainWindowViewModel
    {
        public ICommand DeleteBackupEntryCommand { get; }
        public ICommand SelectStorageDirectoryCommand { get; }
        public ICommand RefreshDevicesCommand { get; }
        public ICommand AddFolderCommand { get; }
        public ICommand TransferFilesCommand { get; }

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

        private PhoneDirectory m_SelectedPhoneDirectory;
        public PhoneDirectory SelectedPhoneDirectory
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

        private ObservableCollection<PhoneDirectory> m_PhoneDirectories;
        public ObservableCollection<PhoneDirectory> PhoneDirectories
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

        private double _progressValue;
        public double ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        private string _progressText;
        public string ProgressText
        {
            get { return _progressText; }
            set
            {
                _progressText = value;
                OnPropertyChanged(nameof(ProgressText));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand ShowMessageCommand { get; private set; }

        public MainWindowViewModel()
        {
            DeleteBackupEntryCommand = new RelayCommand<BackupEntry>(DeleteBackEntry);
            RefreshDevicesCommand = new RelayCommand<MainWindowViewModel>(RefreshDevices);
            AddFolderCommand = new RelayCommand<PhoneDirectory>(AddFolder);
            TransferFilesCommand = new RelayCommand<BackupEntry>(TransferFiles);

            m_Phones = new ObservableCollection<Phone>();
            m_PhoneDirectories = new ObservableCollection<PhoneDirectory>();
            m_BackupEntries = new ObservableCollection<BackupEntry>();

            m_PhoneDirectories.Add(new PhoneDirectory() { Name = "Test", Path = "Test", NumberOfFFiles = 77 });
            m_PhoneDirectories.Add(new PhoneDirectory() { Name = "Test1", Path = "Test1", NumberOfFFiles = 203 });
            m_PhoneDirectories.Add(new PhoneDirectory() { Name = "Test2", Path = "Test2", NumberOfFFiles = 154 });
        }

        internal void RefreshDevices(object parameter)
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

        internal void ReadFolders(MediaDevice device)
        {
            PhoneDirectories.Clear();
            var directories = device.EnumerateDirectories(device.GetRootDirectory().FullName);
            foreach (var directory in directories)
            {
                var subDirectories = device.EnumerateDirectories(System.IO.Path.GetFileName(directory));
                foreach (var subDirectory in subDirectories)
                {
                    var subDirectoryInfo = device.GetDirectoryInfo(subDirectory);
                    PhoneDirectories.Add(new PhoneDirectory() { Path = subDirectory, Name = Path.GetFileName(subDirectory), NumberOfFFiles = subDirectoryInfo.EnumerateFiles("*.*", SearchOption.AllDirectories).Count(), Id = Guid.NewGuid() });
                }
            }
        }

        internal void AddFolder(object parameter)
        {
            if (parameter is PhoneDirectory phoneDirectory)
            {
                if (BackupEntries.FirstOrDefault(x => x.SourceDirectory == phoneDirectory.Path) == null)
                {
                    BackupEntries.Add(new BackupEntry() { Phone = SelectedPhone, SourceDirectory = phoneDirectory.Path, DestinationDirectory = "c:/backuptest/", Id = phoneDirectory.Id});
                }
                // todo notify user that folder already exists
                phoneDirectory.IsAdded = true;
            }
        }

        internal void DeleteBackEntry(BackupEntry backupEntry)
        {
            if (backupEntry != null && BackupEntries.Contains(backupEntry))
            {
                BackupEntries.Remove(backupEntry);
                var phoneDirectory = PhoneDirectories.FirstOrDefault(x => x.Id == backupEntry.Id);
                if (phoneDirectory != null)
                {
                    phoneDirectory.IsAdded = false;
                }
            }
        }

        private async void TransferFiles(BackupEntry backupEntry)
        {
            if (backupEntry == null) return;

            var device = MediaDevice.GetDevices().FirstOrDefault(d => d.DeviceId == backupEntry.Phone.DeviceId);
            if (device == null)
            {
                ShowNotification("Device not found.");
                return;
            }

            device.Connect();

            var sourceDirectory = backupEntry.SourceDirectory;
            var destinationDirectory = backupEntry.DestinationDirectory;

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            await Task.Run(() =>
            {
                var files = device.EnumerateFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);
                int totalFiles = files.Count();
                int filesCopied = 0;

                foreach (var file in files)
                {
                    var fileInfo = device.GetFileInfo(file);
                    var destinationPath = Path.Combine(destinationDirectory, fileInfo.FullName.Substring(sourceDirectory.Length + 1));
                    var destinationDir = Path.GetDirectoryName(destinationPath);
                    if (!Directory.Exists(destinationDir))
                    {
                        Directory.CreateDirectory(destinationDir);
                    }

                    using (var sourceStream = fileInfo.OpenRead())
                    using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                    {
                        sourceStream.CopyTo(destinationStream);
                    }

                    filesCopied++;


                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProgressText = $"Copying file {filesCopied} of {totalFiles}: {fileInfo.Name}";
                        ProgressValue = (double)filesCopied / totalFiles * 100;
                    });
                }
            });

            device.Disconnect();
            ShowNotification("Files copied successfully");
        }
        private void ShowNotification(string message)
        {
            MessageBox.Show(message, "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
