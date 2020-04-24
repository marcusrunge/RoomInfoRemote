namespace RoomInfoRemote.Models
{
    public enum OccupancyVisualState { FreeVisualState, PresentVisualState, AbsentVisualState, BusyVisualState, OccupiedVisualState, LockedVisualState, HomeVisualState, UndefinedVisualState }
    public enum PayloadType { Occupancy, Room, Schedule, StandardWeek, RequestOccupancy, RequestSchedule, RequestStandardWeek, IotDim, AgendaItem, AgendaItemId, Discovery, PropertyChanged, TimeSpanItem, TimeSpanItemId }
    public enum NetworkProtocol { UserDatagram, TransmissionControl }
    public enum ButtenType { Refresh }
    public enum Theme { Default, Light, Dark }
}
