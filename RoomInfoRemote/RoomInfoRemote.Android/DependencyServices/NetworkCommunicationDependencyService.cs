using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.Models;
using Xamarin.Forms;
using RoomInfoRemote.Droid.DependencyServices;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;

[assembly: Dependency(typeof(NetworkCommunicationDependencyService))]
namespace RoomInfoRemote.Droid.DependencyServices
{
    public class NetworkCommunicationDependencyService : INetworkCommunication
    {
        public event EventHandler<PayloadReceivedEventArgs> PayloadReceived;

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
            TcpListener tcpListener = new TcpListener(IPAddress.Any, int.Parse(port));
            tcpListener.Start();
            while (true)
            {
                try
                {
                    using (TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync())
                    {
                        using (NetworkStream networkStream = tcpClient.GetStream())
                        {
                            StreamReader streamReader = new StreamReader(networkStream, Encoding.UTF8);
                            string response = await streamReader.ReadLineAsync();
                            OnPayloadReceived(new PayloadReceivedEventArgs(((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString(), response));
                            streamReader.Close();
                            networkStream.Close();
                        }

                        tcpClient.Close();
                    }
                }
                catch { }
            }

        }

        private async Task ListenForUserDatagramConnection(string port)
        {
            UdpClient udpClient = new UdpClient(int.Parse(port));
            while (true)
            {
                try
                {
                    UdpReceiveResult received = await udpClient.ReceiveAsync();
                    OnPayloadReceived(new PayloadReceivedEventArgs(received.RemoteEndPoint.Port.ToString(), Encoding.ASCII.GetString(received.Buffer)));
                }
                catch { }
            }
        }

        private async Task SendUserDatagramPayload(string payload, string hostName, string port, bool broadcast)
        {
            try
            {
                UdpClient udpClient = new UdpClient();
                byte[] bytes = Encoding.ASCII.GetBytes(payload);
                if (broadcast) hostName = "255.255.255.255";
                await udpClient.SendAsync(bytes, bytes.Length, hostName, int.Parse(port));
                udpClient.Close();
            }
            catch { }
        }

        private async Task SendTransmissionControlPayload(string payload, string hostName, string port)
        {
            try
            {
                using (TcpClient tcpClient = new TcpClient())
                {
                    await tcpClient.ConnectAsync(hostName, int.Parse(port));
                    NetworkStream networkStream = tcpClient.GetStream();
                    byte[] payloadAsBytes = Encoding.UTF8.GetBytes(payload);
                    await networkStream.WriteAsync(payloadAsBytes, 0, payloadAsBytes.Length);
                    await networkStream.FlushAsync();
                    MemoryStream memoryStream = new MemoryStream();
                    payloadAsBytes = new byte[1024];
                    int read = 0;
                    do
                    {
                        read = await networkStream.ReadAsync(payloadAsBytes, 0, 1024);
                        memoryStream.Write(payloadAsBytes, 0, read);
                        Array.Clear(payloadAsBytes, 0, payloadAsBytes.Length);
                    } while (read > 0);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    StreamReader streamReader = new StreamReader(memoryStream);
                    OnPayloadReceived(new PayloadReceivedEventArgs(((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port.ToString(), response));                    
                }
                //TcpClient tcpClient = new TcpClient(hostName, int.Parse(port));
                //byte[] bytes = Encoding.ASCII.GetBytes(payload);
                //NetworkStream networkStream = tcpClient.GetStream();
                //await networkStream.WriteAsync(bytes, 0, bytes.Length);
                //StreamReader streamReader = new StreamReader(networkStream, Encoding.UTF8);
                //string response = await streamReader.ReadLineAsync();
                //if (!string.IsNullOrEmpty(response)) OnPayloadReceived(new PayloadReceivedEventArgs(((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port.ToString(), response));
                //streamReader.Close();
                //networkStream.Close();
                //tcpClient.Close();
            }
            catch { }
        }

        void OnPayloadReceived(PayloadReceivedEventArgs e) => PayloadReceived?.Invoke(null, e);
    }
}