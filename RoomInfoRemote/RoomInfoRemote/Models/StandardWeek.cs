
using Prism.Mvvm;

namespace RoomInfoRemote.Models
{
    public class StandardWeek : BindableBase
    {
        int _id = default(int);
        public int Id { get => _id; set { SetProperty(ref _id, value); } }
    }
}
