using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using RoomInfoRemote.UWP.DependencyServices;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Xamarin.Forms;

[assembly: Dependency(typeof(NetworkCommunicationDependencyService))]
namespace RoomInfoRemote.UWP.DependencyServices
{
    public class NetworkCommunicationDependencyService : INetworkCommunication
    {
        StreamSocketListener _streamSocketListener;
        DatagramSocket _datagramSocket;
        public event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;

        public NetworkCommunicationDependencyService()
        {
            System.Diagnostics.Debug.WriteLine("NetworkCommunicationDependencyService Constructor called...");
        }

        public async Task SendPayload(string payload, string hostName, string port, NetworkProtocol networkProtocol, bool broadcast = false)
        {
            switch (networkProtocol)
            {
                case NetworkProtocol.UserDatagram:
                    await SendUserDatagramPayload(payload, hostName, port, broadcast);
                    break;
                case NetworkProtocol.TransmissionControl:
                    await SendTransmissionControlPayload(payload, hostName, port);
                    break;
                default:
                    break;
            }
        }

        private async Task SendUserDatagramPayload(string payload, string hostName, string port, bool broadcast)
        {
            try
            {
                using (var datagramSocket = new DatagramSocket())
                {
                    if (broadcast) hostName = "255.255.255.255";
                    await datagramSocket.ConnectAsync(new HostName(hostName), port);
                    using (Stream outputStream = datagramSocket.OutputStream.AsStreamForWrite())
                    {
                        using (var streamWriter = new StreamWriter(outputStream))
                        {
                            await streamWriter.WriteLineAsync(payload);
                            await streamWriter.FlushAsync();
                        }
                    }
                    datagramSocket.Dispose();
                }
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
            }
        }

        private async Task SendTransmissionControlPayload(string payload, string hostName, string port)
        {
            try
            {
                using (var streamSocket = new StreamSocket())
                {
                    await streamSocket.ConnectAsync(new HostName(hostName), port);
                    using (Stream outputStream = streamSocket.OutputStream.AsStreamForWrite())
                    {
                        using (var streamWriter = new StreamWriter(outputStream))
                        {
                            await streamWriter.WriteLineAsync(payload);
                            await streamWriter.FlushAsync();
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
            }
        }

        public async Task StartConnectionListener(string port, NetworkProtocol networkProtocol)
        {
            switch (networkProtocol)
            {
                case NetworkProtocol.UserDatagram:
                    await ListenForUserDatagramConnection(port);
                    break;
                case NetworkProtocol.TransmissionControl:
                    await ListenForTransmissionControlConnection(port);
                    break;
                default:
                    break;
            }
        }

        private async Task ListenForTransmissionControlConnection(string port)
        {
            try
            {
                _streamSocketListener = new StreamSocketListener();
                _streamSocketListener.ConnectionReceived += async (s, e) =>
                {
                    using (Stream inputStream = e.Socket.InputStream.AsStreamForRead())
                    {
                        using (StreamReader streamReader = new StreamReader(inputStream))
                        {                            
                            OnConnectionReceived(new ConnectionReceivedEventArgs(e.Socket.Information.RemotePort, await streamReader.ReadLineAsync()));
                        }
                    }
                    e.Socket.Dispose();
                };
                await _streamSocketListener.BindServiceNameAsync(port);
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
            }
        }

        private async Task ListenForUserDatagramConnection(string port)
        {
            try
            {
                _datagramSocket = new DatagramSocket();
                _datagramSocket.MessageReceived += (s, e) =>
                {                    
                    uint stringLength = e.GetDataReader().UnconsumedBufferLength;
                    OnConnectionReceived(new ConnectionReceivedEventArgs(e.RemotePort, e.GetDataReader().ReadString(stringLength)));
                };
                await _datagramSocket.BindServiceNameAsync(port);
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
            }
        }

        void OnConnectionReceived(ConnectionReceivedEventArgs e) => ConnectionReceived?.Invoke(null, e);
    }
}
