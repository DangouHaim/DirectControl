using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace DAL
{
    public interface INetworkService : IDisposable
    {
        event EventHandler<ConnectionsChangedEventArgs> OnConnectionsChanged;
        void StartServer();
        bool ConnectToServer();
        void Disconnect();
    }
}
