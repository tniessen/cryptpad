using cryptpad.Properties;
using System;
using System.ComponentModel;
using System.Windows;

namespace cryptpad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string StartupFilePath;

        public App()
        {
            InitializeComponent();

            // Automatically save settings on change
            Settings.Default.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                Settings.Default.Save();
            });

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                StartupFilePath = args[1];
            }
            else
            {
                StartupFilePath = null;
            }
        }
    }
}
