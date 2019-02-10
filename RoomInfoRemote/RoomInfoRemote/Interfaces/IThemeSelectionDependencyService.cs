using RoomInfoRemote.Models;

namespace RoomInfoRemote.Interfaces
{
    public interface IThemeSelectionDependencyService
    {
        void SetTheme(Theme theme);
        Theme GetTheme();
    }
}
