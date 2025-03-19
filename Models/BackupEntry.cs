using System.ComponentModel;

namespace PhoneBackup.Models
{
    public class BackupEntry : INotifyPropertyChanged
    {
        private Guid _id;
        private Phone _phone;
        private string _sourceDirectory;
        private string _destinationDirectory;
        private bool _inSync;
        private bool _isAvailable;

        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public Phone Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        public string SourceDirectory
        {
            get { return _sourceDirectory; }
            set
            {
                _sourceDirectory = value;
                OnPropertyChanged(nameof(SourceDirectory));
            }
        }

        public string DestinationDirectory
        {
            get { return _destinationDirectory; }
            set
            {
                _destinationDirectory = value;
                OnPropertyChanged(nameof(DestinationDirectory));
            }
        }

        public bool InSync
        {
            get { return _inSync; }
            set
            {
                _inSync = value;
                OnPropertyChanged(nameof(InSync));
            }
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
            set
            {
                _isAvailable = value;
                OnPropertyChanged(nameof(IsAvailable));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}