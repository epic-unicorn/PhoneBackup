using FolderNavigationDemo;
using MediaDevices;
using PhoneBackup.Commands;
using PhoneBackup.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace PhoneBackup.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ICommand m_RefreshDevicesCommand;
        public ICommand RefreshDevicesCommand
        {
            get { return m_RefreshDevicesCommand; }
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

        private List<Phone> m_Phones;
        public List<Phone> Phones
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


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowViewModel()
        {
            m_RefreshDevicesCommand = new RefreshDevicesCommand(this);
            m_Phones = new List<Phone>();
            m_PhoneDirectories = new ObservableCollection<PhoneDirectory>();
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
                    PhoneDirectories.Add(new PhoneDirectory() { Name = System.IO.Path.GetFileName(subDirectory) });
                }
            }
        }
    }
}
