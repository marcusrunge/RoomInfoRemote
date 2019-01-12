using System;

namespace RoomInfoRemote.Models
{
    public class ConnectionReceivedEventArgs : EventArgs
    {
        public ConnectionReceivedEventArgs(string hostName, string package)
        {
            HostName = hostName;
            Package = package;
        }
        public string HostName { get; }
        public string Package { get; }
    }
}
