using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace FolderNavigationDemo
{
    public class PhoneDirectory : INotifyPropertyChanged
    {
        private Guid _id;
        private string _name;
        private string _path;
        private int _numberOfFiles;
        private bool _isAdded;

        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }   
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged(nameof(Path));
            }
        }

        public int NumberOfFFiles
        {
            get { return _numberOfFiles; }
            set
            {
                _numberOfFiles = value;
                OnPropertyChanged(nameof(NumberOfFFiles));
            }
        }

        public bool IsAdded
        {
            get { return _isAdded; }
            set
            {
                _isAdded = value;
                OnPropertyChanged(nameof(IsAdded));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

}