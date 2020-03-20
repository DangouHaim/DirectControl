using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class NetworkService : INetworkService
    {
        private TcpListener _server;
        private ObservableCollection<TcpClient> _connections;

        public event EventHandler<ConnectionsChangedEventArgs> OnConnectionsChanged = (s, e) => { };

        private int _port { get; set; }
        private bool _serverStarted { get; set; }

        public NetworkService(int port)
        {
            _port = port;
            _connections = new ObservableCollection<TcpClient>();

            _connections.CollectionChanged += (s, e) =>
            {
                OnConnectionsChanging();
            };
            
            UpdateConnections();
        }

        private void OnConnectionsChanging()
        {
            OnConnectionsChanged.Invoke(this, new ConnectionsChangedEventArgs()
            {
                Connections = _connections
            });
        }

        private void UpdateConnections()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    List<TcpClient> toRemove = (from x in _connections
                                                where x == null || !x.Connected
                                                select x).ToList();

                    for (int i = 0; i < toRemove.Count(); i++)
                    {
                        _connections.Remove(toRemove[i]);
                    }

                    Task.Delay(1000).Wait();
                }
            });
        }

        public bool ConnectToServer()
        {
            TcpClient client = NetworkHelper.ConnectToServer(_port, 80);
            
            if (client == null)
                return false;

            _connections.Add(client);

            return true;
        }

        public void Disconnect()
        {
            _serverStarted = false;
            _server?.Stop();

            foreach (var v in _connections)
                v.Close();
            
            _connections.Clear();
        }

        public void Dispose()
        {
            Disconnect();
        }

        public void StartServer()
        {
            _server = new TcpListener(IPAddress.Any, _port);
            _server.Start();
            
            _serverStarted = true;

            Task.Run(() =>
            {
                while(_serverStarted)
                {
                    _connections.Add(_server.AcceptTcpClient());
                }
            });
        }
    }
}
