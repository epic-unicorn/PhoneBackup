using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MediaDevices;

namespace PhoneBackup
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnReadFolders_Click(object sender, RoutedEventArgs e)
        {
            treeViewFolders.Items.Clear();
            var devices = MediaDevice.GetDevices();
            foreach (var device in devices)
            {
                device.Connect();
                var rootItem = new TreeViewItem { Header = device.Description };
                treeViewFolders.Items.Add(rootItem);
                ReadFolders(device, rootItem, device.GetRootDirectory().FullName);
                device.Disconnect();
            }
        }

        private void ReadFolders(MediaDevice device, TreeViewItem parentItem, string path)
        {
            var directories = device.EnumerateDirectories(path);
            foreach (var directory in directories)
            {
                var directoryItem = new TreeViewItem { Header = Path.GetFileName(directory) };
                parentItem.Items.Add(directoryItem);
                ReadFolders(device, directoryItem, directory);
            }
        }
    }
}

