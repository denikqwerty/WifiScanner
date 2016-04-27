using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WifiScanner.NetFolders
{
    public class NetworkMain
    {
        public string GetHostName(string ipAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                if (entry != null)
                {
                    return entry.HostName;
                }
            }
            catch (SocketException)
            {
                // MessageBox.Show(e.Message.ToString());
            }

            return null;
        }


        //Get MAC address
        public string GetMacAddress(string ipAddress)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    FileName = "arp",
                    Arguments = "-a " + ipAddress,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string strOutput = process.StandardOutput.ReadToEnd();
            string[] substrings = strOutput.Split('-');
            if (substrings.Length >= 8)
            {
                var macAddress = (substrings[3].Substring(Math.Max(0, substrings[3].Length - 2))
                                     + "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6]
                                     + "-" + substrings[7] + "-"
                                     + substrings[8].Substring(0, 2)).ToUpper();
                return macAddress;
            }

            else
            {
                foreach (NetworkInterface f in from f in NetworkInterface.GetAllNetworkInterfaces() where f.OperationalStatus == OperationalStatus.Up from d in f.GetIPProperties().GatewayAddresses select f)
                {
                    var mac = f.GetPhysicalAddress().ToString();
                    for (int i = 2; i < mac.Length; i = i + 3)
                    {
                        mac = mac.Insert(i, "-");
                    }
                    return mac;
                }
                return null;
            }
        }

        public string NetworkGateway()
        {
            return (from f in NetworkInterface.GetAllNetworkInterfaces() where f.OperationalStatus == OperationalStatus.Up from d in f.GetIPProperties().GatewayAddresses select d.Address.ToString()).FirstOrDefault();
        }
    }
}
