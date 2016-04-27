using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;

namespace WifiScanner
{
    /// <summary>
    /// Interaction logic for Ping_host.xaml
    /// </summary>
    public partial class PingHost
    {
        public PingHost()
        {
            InitializeComponent();

            string gateIp = NetworkGateway();

            //Extracting and pinging all other ip's.
            string[] array = gateIp.Split('.');

            txtfirst.Text = array[0];
            txtsecond.Text = array[1];
            txtthird.Text = array[2];
            txtfourth.Text = array[3];

            txtattempts.Text = "4";
            txttimeout.Text = "4000";
        }
        
        private void pingbtn_Click(object sender, RoutedEventArgs e)
        {
            int first;
            int second;
            int third;
            int fourth;
            if (txtfirst.Text.Length == 0 && Int32.TryParse(txtfirst.Text, out first))
            {
                MessageBox.Show("All fields should contain values");
            }
            else if (txtsecond.Text.Length == 0 && Int32.TryParse(txtsecond.Text, out second))
            {
                MessageBox.Show("All fields should contain values");
            }
            else if (txtthird.Text.Length == 0 && Int32.TryParse(txtthird.Text, out third))
            {
                MessageBox.Show("All fields should contain values");
            }
            else if (txtfourth.Text.Length == 0 && Int32.TryParse(txtfourth.Text, out fourth))
            {
                MessageBox.Show("All fields should contain values");
            }
            else
            {
                int.TryParse(txtfirst.Text, out first);
                int.TryParse(txtsecond.Text, out second);
                int.TryParse(txtthird.Text, out third);
                int.TryParse(txtfourth.Text, out fourth);

                string hostAddress = first + "." + second + "." + third + "." + fourth;
                int attempts = 4;
                Int32.TryParse(txtattempts.Text, out attempts);
                int timeout = 4000;
                Int32.TryParse(txttimeout.Text, out timeout);
                Ping(hostAddress, attempts, timeout);
            }
        }

        private void Ping(string host, int attempts, int timeout)
        {
            txtblckresult.Text += "Pinging host " + host + ": \n";
            for (int i = 0; i < attempts; i++)
            {
                new Thread(delegate ()
                {
                    try
                    {
                        Ping ping = new Ping();
                        ping.PingCompleted += PingCompletedCallback;
                        ping.SendAsync(host, timeout, host);
                    }
                    catch
                    {
                        // Do nothing and let it try again until the attempts are exausted.
                        // Exceptions are thrown for normal ping failurs like address lookup
                        // failed.  For this reason we are supressing errors.
                    }
                }).Start();
            }
        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            // If an error occurred, display the exception to the user. 
            if (e.Reply.Status == IPStatus.Success)
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        txtblckresult.Text += "Success! \n";
                    });
                }
            }
            else {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        txtblckresult.Text += "Fail! \n";
                    });
                }
            }
        }

        private string NetworkGateway()
        {
            return (from f in NetworkInterface.GetAllNetworkInterfaces() where f.OperationalStatus == OperationalStatus.Up from d in f.GetIPProperties().GatewayAddresses select d.Address.ToString()).FirstOrDefault();
        }
    }
}
