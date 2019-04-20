using Prism.Events;
using System;

namespace RoomInfoRemote.Models
{
    public class CurrentPageChangedEventArgs : EventArgs
    {
        public Type PageType { get; set; }
        public string HostName { get; set; }
        public CurrentPageChangedEventArgs(Type pageType, string hostName)
        {
            PageType = pageType;
            HostName = hostName;
        }
    }
    public class CurrentPageChangedEvent : PubSubEvent<CurrentPageChangedEventArgs> { }    
}
