using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using RoomInfoRemote.Interfaces;
using RoomInfoRemote.iOS.DependencyServices;
using RoomInfoRemote.Models;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(NetworkCommunicationDependencyService))]
namespace RoomInfoRemote.iOS.DependencyServices
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
            try
            {

            }
            catch (Exception)
            {

            }
        }

        private async Task ListenForUserDatagramConnection(string port)
        {
            try
            {

            }
            catch (Exception)
            {

            }
        }

        private async Task SendUserDatagramPayload(string payload, string hostName, string port, bool broadcast)
        {
            try
            {

            }
            catch (Exception)
            {

            }
        }

        private async Task SendTransmissionControlPayload(string payload, string hostName, string port)
        {
            try
            {

            }
            catch (Exception)
            {

            }
        }

        void OnPayloadReceived(PayloadReceivedEventArgs e) => PayloadReceived?.Invoke(null, e);
    }
}