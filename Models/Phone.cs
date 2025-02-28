using System.ComponentModel;

namespace PhoneBackup.Models
{
    public class Phone : INotifyPropertyChanged
    {       
        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set
            {
                if (value != m_Name)
                {
                    m_Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string m_Vendor;
        public string Vendor
        {
            get { return m_Vendor; }
            set
            {
                if (value != m_Vendor)
                {
                    m_Vendor = value;
                    OnPropertyChanged(nameof(Vendor));
                }
            }
        }

        private string m_DeviceId;
        public string DeviceId
        {
            get { return m_DeviceId; }
            set
            {
                if (value != m_DeviceId)
                {
                    m_DeviceId = value;
                    OnPropertyChanged(nameof(DeviceId));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
