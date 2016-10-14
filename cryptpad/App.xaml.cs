using System;
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

            string[] args = Environment.GetCommandLineArgs();
            if(args.Length > 1)
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
