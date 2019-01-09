namespace RoomInfoRemote.Models
{
    public enum OccupancyVisualState { FreeVisualState, PresentVisualState, AbsentVisualState, BusyVisualState, OccupiedVisualState, LockedVisualState, UndefinedVisualState }
    public enum PayloadType { Occupancy, Room, Schedule, StandardWeek, RequestOccupancy, RequestSchedule, RequestStandardWeek, IotDim }
}
