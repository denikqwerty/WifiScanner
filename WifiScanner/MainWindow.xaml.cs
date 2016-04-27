using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Net.NetworkInformation;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using WifiScanner.NetFolders;

namespace WifiScanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private class InfoItem
        {
            public string IP { get; set; }
            public string MAC { get; set; }
            public string HOST { get; set; }
        }
        List<InfoItem> _unsortedList; //unsorted list of successfull pings
        bool _stopFlag;
        private List<InfoItem> _sortedList; // sorted list of completed pings
        private NetworkMain networkMain = new NetworkMain();

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            if (_stopFlag)
                _stopFlag = false; // renew the stopflag

            pbStatus.Value = 0; // renew the progress bar
            pbStatus.Maximum = 100;

            string gateIp = networkMain.NetworkGateway(); //get own gateway
            //Extracting and pinging all other ip's.

            _unsortedList = new List<InfoItem>(); //initialize new unsorted list of complete pings


            string[] array = gateIp.Split('.');
            try
            {
                for (int i = 1; i <= 255; i++)
                {
                    string pingVar = array[0] + "." + array[1] + "." + array[2] + "." + i;

                    new Thread(delegate () {

                        //time in milliseconds           
                        Ping(pingVar, 1, 4000);
                    }).Start();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void Ping(string host, int attempts, int timeout)
        {
            try
            {
                var ping = new Ping();

                ping.PingCompleted += PingCompletedCallback;
                ping.SendAsync(host, timeout, host); // send packets asyncronusly

                //work with progress bar
                ping.PingCompleted += delegate
                {
                    if (!Dispatcher.CheckAccess())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            pbStatus.Value++;
                        });
                    }
                };
            }
            catch
            {
                // Do nothing and let it try again until the attempts are exausted.
                // Exceptions are thrown for normal ping failurs like address lookup
                // failed.  For this reason we are supressing errors.
            }
        }


        public void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            if (_stopFlag)
                return;
            // If an error occurred, display the exception to the user. 
            if (e.Reply.Status == IPStatus.Success)
            {

                string hostName = networkMain.GetHostName(e.Reply.Address.ToString());
                string macAdress = networkMain.GetMacAddress(e.Reply.Address.ToString());

                if (_stopFlag)
                    return;

                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (_stopFlag)
                            return;
                        if (_unsortedList.Find(x => x.IP == e.Reply.Address.ToString()) != null)
                        {
                            return;
                        }
                        _unsortedList.Add(new InfoItem() { IP = e.Reply.Address.ToString(), MAC = macAdress, HOST = hostName });
                        _sortedList = _unsortedList.OrderBy(item => Version.Parse(item.IP)).ToList();
                        lstNetworks.Items.Clear();
                        
                        foreach (var item in _sortedList)
                        {
                            if (_stopFlag)
                                return;
                            lstNetworks.Items.Add(item);
                        }
                    });
                }
            }
            else {
                //Console.WriteLine(String.Concat("Non-active IP: ", e.Reply.Address.ToString()))
            }
        }

        private void sharedbtn_Click(object sender, RoutedEventArgs e)
        {
            _stopFlag = true;
            refreshbtn.IsEnabled = false;

            if (_sortedList == null)
            {
                MessageBox.Show("You should click Refresh button first");
                refreshbtn.IsEnabled = true;
                return;
            }
            if (_sortedList.Count != 0)
            {
                pbStatus.Value = 0; // renew the progress bar
                pbStatus.Maximum = _sortedList.Count;

                if (lstNetworks.Items.Count != _sortedList.Count)
                {
                    lstNetworks.Items.Clear();
                    foreach (var item in _sortedList)
                    {
                        lstNetworks.Items.Add(item);
                    }
                }

                try
                {
                    foreach (var item in _sortedList)
                    {
                        new Thread(delegate ()
                        {
                            List<Share> lstShares = TestShares.SharedFolders(item.IP);
                            if (lstShares.Count != 0)
                            {
                                foreach (var si in lstShares)
                                {
                                    int counter = lstNetworks.Items.IndexOf(item) + 1;
                                    if (!Dispatcher.CheckAccess())
                                    {
                                        Dispatcher.Invoke(() =>
                                        {
                                            lstNetworks.Items.Insert(counter,
                                                new InfoItem() { IP = "", MAC = si.ToString(), HOST = "" });
                                            pbStatus.Value++;
                                        });
                                    }
                                }
                            }
                            if (item == _sortedList.Last())
                            {
                                if (!Dispatcher.CheckAccess())
                                {

                                    Dispatcher.Invoke(() =>
                                    {
                                        refreshbtn.IsEnabled = true;
                                    });
                                }
                            }
                        }).Start();
                    }

                }
                catch
                {
                    // Do nothing and let it try again until the attempts are exausted.
                    // Exceptions are thrown for normal ping failurs like address lookup
                    // failed.  For this reason we are supressing errors.
                }


            }
        }

        private void lstNetworks_Click(object sender, RoutedEventArgs e)
        {
            ListView listView = sender as ListView;
            var item = listView?.SelectedItem as InfoItem;
            if (item != null)
            {
                if (item.IP == "")
                {
                    var path = item.MAC;
                    System.Diagnostics.Process.Start("explorer.exe", path);
                }
                else
                {
                    var path = "\\\\" + item.IP;
                    System.Diagnostics.Process.Start("explorer.exe", path);
                }
            }
        }

        private void stopbtn_Click(object sender, RoutedEventArgs e)
        {
            _stopFlag = true;

        }
        private void pinghostbtn_Click(object sender, RoutedEventArgs e)
        {
            PingHost wind = new PingHost();
            wind.ShowDialog();
        }
        private void savebtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog
            {
                FileName = "DefaultOutputName.txt",
                Filter = "Text File | *.txt"
            };
            if (save.ShowDialog() == true)
            {
                StreamWriter writer = new StreamWriter(save.OpenFile());
                foreach (InfoItem item in lstNetworks.Items)
                {
                    writer.WriteLine(item.IP + "      " + item.MAC + "        " + item.HOST);
                }
                writer.Dispose();
                writer.Close();
            }
        }

        private void exitbtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tracertbtn_Click(object sender, RoutedEventArgs e)
        {
            TracertWpf wind = new TracertWpf();
            wind.ShowDialog();
        }

    }
}

