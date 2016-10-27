using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cryptpad
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void HomepageLinkClicked(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                Close();
            }
        }

        private AssemblyName AppAssemblyName
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName();
            }
        }

        public string AppTitle
        {
            get
            {
                return AppAssemblyName.Name;
            }
        }

        public string AppVersion
        {
            get
            {
                return AppAssemblyName.Version.ToString();
            }
        }

        public string AppBuildDate
        {
            get
            {
                Version v = Assembly.GetExecutingAssembly().GetName().Version;
                DateTime dt = new DateTime(2000, 1, 1).AddDays(v.Build).AddSeconds(v.Revision * 2);
                string format = Properties.Resources.AboutBuildTime;
                return string.Format(format, dt.ToString("d"), dt.ToString("T"));
            }
        }

        public Uri AppHomepageUri
        {
            get
            {
                return new Uri(Properties.Resources.AboutHomepage);
            }
        }
    }
}
