using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ScreenShareService : IScreenShareService
    {
        private INetworkService _networkService;

        public bool IsStarted { get; private set; }

        public event EventHandler<ScreenShareEventArgs> OnFrame = (s, e) => { };

        public ScreenShareService(INetworkService networkService)
        {
            _networkService = networkService;
        }

        public void StartReceive()
        {
            IsStarted = true;

            TcpClient client = null;

            _networkService.OnConnectionsChanged += (s, e) =>
            {
                client = e.Connections.FirstOrDefault();
            };

            if(_networkService.ConnectToServer())
            {
                if(client != null)
                {
                    IFormatter formatter = new BinaryFormatter();

                    Task.Factory.StartNew(() =>
                    {
                        while (IsStarted)
                        {
                            OnFrame.Invoke(this, new ScreenShareEventArgs()
                            {
                                Capture = formatter.Deserialize(client.GetStream()) as byte[]
                            });
                        }
                    });
                }
            }
        }

        public void StartShare(IScreenShotService screenShotService)
        {
            IsStarted = true;

            _networkService.StartServer();

            IFormatter formatter = new BinaryFormatter();

            List<TcpClient> connections = null;

            _networkService.OnConnectionsChanged += (s, e) =>
            {
                connections = e.Connections.ToList();
            };

            Task.Factory.StartNew(() =>
            {
                while (IsStarted)
                {
                    if (connections == null || !connections.Any())
                    {
                        Task.Delay(1000).Wait();
                        continue;
                    }

                    for (int i = 0; i < connections.Count; i++)
                    {
                        TcpClient client = connections[i];
                        
                        if (client != null && client.Connected)
                        {
                            try
                            {
                                formatter.Serialize(client.GetStream(), screenShotService.Capture());
                            }
                            catch { }
                        }
                    }
                }
            });
        }

        public void Stop()
        {
            IsStarted = false;
            _networkService.Disconnect();
        }
    }
}
