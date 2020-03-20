using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace DAL
{
    public class ConnectionsChangedEventArgs : EventArgs
    {
        public IEnumerable<TcpClient> Connections { get; set; }
    }
}
