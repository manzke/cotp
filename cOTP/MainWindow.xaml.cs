using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Threading;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.Configuration;

namespace Devsurf.Security.Otp.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MotpProvider provider = new MotpProvider();
        private DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void cOTP_Loaded(object sender, RoutedEventArgs e)
        {
            String secret = "";
            AppSettingsReader reader = new AppSettingsReader();

            secret = reader.GetValue("Secret", secret.GetType()).ToString();
            otpBox.DataContext = provider;
            provider.NotifyPropertyChanged("Now");

            PinDialog pinDialog = new PinDialog();
            pinDialog.Owner = this;

            if (pinDialog.ShowDialog() == true)
            {
                String pin = pinDialog.pinTextBox.Password;
                provider.Pin = pin;
                provider.Secret = secret;

                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }
            if(secret == null || secret.Equals(""))
            {
                MessageBox.Show("Please configure your secret in the cOTP.config");
                Close();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            provider.NotifyPropertyChanged("Now");
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
