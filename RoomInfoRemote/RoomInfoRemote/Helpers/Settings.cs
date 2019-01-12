using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace RoomInfoRemote.Helpers
{
    public static class Settings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public static string TcpPort
        {
            get => AppSettings.GetValueOrDefault(nameof(TcpPort), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(TcpPort), value);
        }

        public static string UdpPort
        {
            get => AppSettings.GetValueOrDefault(nameof(UdpPort), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UdpPort), value);
        }
    }
}
