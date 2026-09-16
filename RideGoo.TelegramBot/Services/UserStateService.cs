using RideGoo.Domain.ValueObjects; // GeoLocation shu yerda bo'lishi kerak — agar boshqa joyda bo'lsa, using'ni moslang

namespace RideGoo.TelegramBot.Services
{
    // Foydalanuvchi hozir qaysi bosqichda ekanini bildiradi
    public enum OrderStage
    {
        None,                     // Hech narsa qilmayapti
        WaitingPickupLocation,    // "Qayerdan" joylashuvini kutmoqda
        WaitingDestinationLocation // "Qayerga" joylashuvini kutmoqda
    }

    // Har bir foydalanuvchi uchun vaqtinchalik ma'lumot (RAM'da saqlanadi)
    public class UserState
    {
        public OrderStage Stage { get; set; } = OrderStage.None;
        public GeoLocation? PickupLocation { get; set; }
        public GeoLocation? DestinationLocation { get; set; }
    }

    // Bu servis barcha foydalanuvchilarning holatini saqlaydi
    // chatId (Telegram chat ID) -> UserState
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