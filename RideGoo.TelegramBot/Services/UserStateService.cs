using RideGoo.Domain.ValueObjects;

namespace RideGoo.TelegramBot.Services
{
    public enum OrderStage
    {
        None,
        WaitingPickupLocation,
        WaitingDestinationLocation
    }

    public class UserState
    {
        public OrderStage Stage { get; set; } = OrderStage.None;
        public GeoLocation? PickupLocation { get; set; }
        public string? PickupAddress { get; set; }
        public GeoLocation? DestinationLocation { get; set; }
        public string? DestinationAddress { get; set; }
    }

    public class UserStateService
    {
        private readonly Dictionary<long, UserState> _states = new();

        public UserState GetOrCreate(long chatId)
        {
            if (!_states.TryGetValue(chatId, out var state))
            {
                state = new UserState();
                _states[chatId] = state;
            }
            return state;
        }

        public void Reset(long chatId)
        {
            _states.Remove(chatId);
        }
    }
}