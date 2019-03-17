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
        public event EventHandler<PayloadReceivedEventArgs> PayloadReceived;

        public NetworkCommunicationDependencyService()
        {
            
        }

        public async Task SendPayload(string payload, string hostName, string port, NetworkProtocol networkProtocol, bool broadcast = false)
        {
            switch (networkProtocol)
            {
                case NetworkProtocol.UserDatagram:
                    if (string.IsNullOrEmpty(port)) port = "8274";
                    await SendUserDatagramPayload(payload, hostName, port, broadcast);
                    break;
                case NetworkProtocol.TransmissionControl:
                    if (string.IsNullOrEmpty(port)) port = "8273";
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
                    using (Stream inputStream = streamSocket.InputStream.AsStreamForRead())
                    {
                        using (StreamReader streamReader = new StreamReader(inputStream))
                        {
                            OnPayloadReceived(new PayloadReceivedEventArgs(streamSocket.Information.RemotePort, await streamReader.ReadLineAsync()));
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
                            OnPayloadReceived(new PayloadReceivedEventArgs(e.Socket.Information.RemoteHostName.CanonicalName, await streamReader.ReadLineAsync()));
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
                    OnPayloadReceived(new PayloadReceivedEventArgs(e.RemoteAddress.CanonicalName, e.GetDataReader().ReadString(stringLength)));
                };
                await _datagramSocket.BindServiceNameAsync(port);
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
            }
        }

        void OnPayloadReceived(PayloadReceivedEventArgs e) => PayloadReceived?.Invoke(null, e);
    }
}
