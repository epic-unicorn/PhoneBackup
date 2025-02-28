using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MediaDevices;
using PhoneBackup.ViewModels;

namespace PhoneBackup
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel m_MainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();
            m_MainWindowViewModel = new MainWindowViewModel();
            DataContext = m_MainWindowViewModel;       
        }  
    }
}

