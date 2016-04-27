using System;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;

namespace WifiScanner
{
    /// <summary>
    /// Interaction logic for Tracert.xaml
    /// </summary>
    public partial class TracertWpf
    {
        public TracertWpf()
        {
            InitializeComponent();
        }

        class TraceItem
        {
            public int HOP { get; set; }
            public string HOST { get; set; }
            public string HOSTNAME { get; set; }
            public string TIME { get; set; }
        }

        private int _hops = 0;
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tracert tracert = new Tracert
                {
                    MaxHops = 30,
                    TimeOut = 5000
                };
                tracert.Done += tracert_Done;
                tracert.RouteNodeFound += tracert_RouteNodeFound;
                tracert.HostNameOrAddress = txtHost.Text;
                lstTracert.Items.Clear();
                tracert.Trace();
                btnStart.IsEnabled = false;
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Tracert Demo");
            }
        }

        delegate void ThreadSwitch();

        private void OnGetHostEntry(IAsyncResult ar)
        {
            try
            {
                TraceItem item = ar.AsyncState as TraceItem;
                    ThreadSwitch delg = delegate {
                        try
                        {
                            var result = Dns.EndGetHostEntry(ar);
                            if (result.HostName != null)
                            {
                                if (item != null)
                                {
                                    item.HOSTNAME = result.HostName;
                                    lstTracert.Items.Add(item);
                                }
                                lstTracert.Items.SortDescriptions.Add(new SortDescription("HOP", ListSortDirection.Ascending));
                            }
                        }
                        catch (Exception)
                        {
                            if (item != null)
                            {
                                item.HOSTNAME = "No name";
                                lstTracert.Items.Add(item);
                            }
                            lstTracert.Items.SortDescriptions.Add(new SortDescription("HOP", ListSortDirection.Ascending));
                        }
                    };
                    Dispatcher.Invoke(delg);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Tracert Demo");
            }
        }

        private void tracert_RouteNodeFound(object sender, RouteNodeFoundEventArgs e)
        {
            TraceItem item = new TraceItem {HOST = e.Node.Address.ToString()};
            _hops += 1;
            item.HOP = _hops;
            item.HOSTNAME = String.Empty;
            item.TIME = e.Node.Status == IPStatus.Success ? e.Node.RoundTripTime.ToString()+" ms" : "*";

            if (e.Node.Status == IPStatus.Success)
            {
                Dns.BeginGetHostEntry(e.Node.Address, OnGetHostEntry, item);
            }
        }

        private void tracert_Done(object sender, EventArgs e)
        {
            btnStart.IsEnabled = true;
        }
    }
}
