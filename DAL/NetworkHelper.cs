using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DAL
{
    internal static class NetworkHelper
    {
        internal static TcpClient ConnectToServer(int port, int timeout = 30)
        {
            TcpClient connection = null;

            foreach (IPAddress ip in GetIPAddresses())
            {
                try
                {
                    TcpClient client = new TcpClient();

                    var result = client.BeginConnect(ip, port, null, null);

                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeout));

                    if (success)
                    {
                        client.EndConnect(result);
                        connection = client;
                        break;
                    }
                    throw new SocketException();
                }
                catch
                {
                    continue;
                }
            }

            return connection;
        }

        internal static IEnumerable<IPAddress> GetIPAddresses()
        {
            List<IPAddress> addresses = new List<IPAddress>();

            // Getting default gateway
            var gateway = GetGatewayAddress();

            string address = gateway.ToString();

            if (!address.StartsWith("127") && !address.StartsWith("0") && address.Contains("."))
            {
                string[] bases = address.Split('.');

                for (int i = int.Parse(bases[bases.Length - 1]); i < 255; i++)
                {
                    IPAddress ipAddress = IPAddress.Parse(address.Remove(address.LastIndexOf('.'))
                        + "." + i.ToString());

                    addresses.Add(ipAddress);
                }
            }
            else
                addresses.Add(IPAddress.Parse("127.0.0.1"));
            return addresses;
        }

        internal static IPAddress GetGatewayAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    string address = ip.ToString();
                    if (!address.StartsWith("127") && !address.StartsWith("0") && address.Contains("."))
                    {
                        address = address.Remove(address.LastIndexOf('.')) + ".0";
                        return IPAddress.Parse(address);
                    }
                }
            }
            return IPAddress.Any;
        }
    }
}
