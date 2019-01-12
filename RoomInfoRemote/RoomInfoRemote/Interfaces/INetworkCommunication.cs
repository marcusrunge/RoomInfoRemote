using RoomInfoRemote.Models;
using System;
using System.Threading.Tasks;

namespace RoomInfoRemote.Interfaces
{
    public interface INetworkCommunication
    {
        event EventHandler<ConnectionReceivedEventArgs> ConnectionReceived;
        Task StartConnectionListener(string port, NetworkProtocol networkProtocol);
        Task SendPayload(string payload, string hostName, string port, NetworkProtocol networkProtocol, bool broadcast = false);
    }
}
