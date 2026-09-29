using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Interfaces;
using RideGoo.Domain.ValueObjects;
using RideGoo.Shared.DTOs.Order;
using RideGoo.Shared.Params;
using RideGoo.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RideGoo.TelegramBot.Handlers;

public class UpdateHandler
{
    private readonly IAuthService _authService;
    private readonly IOrderService _orderService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserStateService _userStateService;
    private readonly GeocodingService _geocodingService;

    public UpdateHandler(
        IAuthService authService,
        IOrderService orderService,
        IUnitOfWork unitOfWork,
        UserStateService userStateService,
        GeocodingService geocodingService)
    {
        _authService = authService;
        _orderService = orderService;
        _unitOfWork = unitOfWork;
        _userStateService = userStateService;
        _geocodingService = geocodingService;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message) return;

        var chatId = message.Chat.Id;

        if (message.Location is not null)
        {
            await HandleLocationAsync(bot, chatId, message.Location, cancellationToken);
            return;
        }

        if (message.Contact is not null)
        {
            await HandleContactAsync(bot, chatId, message.Contact.PhoneNumber, cancellationToken);
            return;
        }

        if (message.Text == "/start")
        {
            var contactKeyboard = new ReplyKeyboardMarkup(new[]
            {
                KeyboardButton.WithRequestContact("📱 Telefon raqamni ulashish")
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            await bot.SendMessage(
                chatId: chatId,
                text: "Assalomu alaykum! RideGoo botiga xush kelibsiz. 🚕\n\nDavom etish uchun pastdagi tugma orqali telefon raqamingizni ulashing.",
                replyMarkup: contactKeyboard,
                cancellationToken: cancellationToken);
            return;
        }

        if (message.Text == "🚕 Buyurtma berish")
        {
            await HandleOrderStartAsync(bot, chatId, cancellationToken);
            return;
        }

        if (message.Text == "📋 Buyurtmalarim")
        {
            await HandleMyOrdersAsync(bot, chatId, cancellationToken);
            return;
        }

        await bot.SendMessage(
            chatId: chatId,
            text: "Buyruqni tushunmadim. /start ni yuboring.",
            cancellationToken: cancellationToken);
    }

    private async Task HandleContactAsync(ITelegramBotClient bot, long chatId, string phoneNumber, CancellationToken cancellationToken)
    {
        var formattedPhone = phoneNumber.StartsWith("+") ? phoneNumber : "+" + phoneNumber;

        var loginResult = await _authService.LoginWithTelegramAsync(formattedPhone, chatId);

        if (loginResult.IsSuccess)
        {
            var mainMenu = BuildMainMenu();

            await bot.SendMessage(
                chatId: chatId,
                text: $"Xush kelibsiz, {loginResult.Data!.FullName}! ✅\n\nSiz tizimga muvaffaqiyatli kirdingiz. Rolingiz: {loginResult.Data.Role}",
                replyMarkup: mainMenu,
                cancellationToken: cancellationToken);
        }
        else
        {
            await bot.SendMessage(
                chatId: chatId,
                text: $"❌ {loginResult.ErrorMessage}\n\nWebsite: https://ridegoo.uz",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }
    }

    private async Task HandleOrderStartAsync(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.TelegramChatId == chatId);

        if (customer is null)
        {
            await bot.SendMessage(
                chatId: chatId,
                text: "Avval tizimga kiring. /start ni yuboring.",
                cancellationToken: cancellationToken);
            return;
        }

        var state = _userStateService.GetOrCreate(chatId);
        state.Stage = OrderStage.WaitingPickupLocation;
        state.PickupLocation = null;
        state.PickupAddress = null;
        state.DestinationLocation = null;
        state.DestinationAddress = null;

        await bot.SendMessage(
            chatId: chatId,
            text: "Qayerdan jo'nayapsiz? 📍\n\nPastdagi tugma orqali joylashuvingizni yuboring.",
            replyMarkup: BuildLocationKeyboard(),
            cancellationToken: cancellationToken);
    }

    private async Task HandleLocationAsync(ITelegramBotClient bot, long chatId, Location location, CancellationToken cancellationToken)
    {
        var state = _userStateService.GetOrCreate(chatId);

        if (state.Stage == OrderStage.None)
        {
            await bot.SendMessage(
                chatId: chatId,
                text: "Joylashuv qabul qilindi, lekin hozir kerak emas edi. Buyurtma berish uchun \"🚕 Buyurtma berish\" tugmasini bosing.",
                cancellationToken: cancellationToken);
            return;
        }

        if (state.Stage == OrderStage.WaitingPickupLocation)
        {
            state.PickupLocation = GeoLocation.Create(location.Latitude, location.Longitude);
            state.PickupAddress = await _geocodingService.GetAddressAsync(location.Latitude, location.Longitude);
            state.Stage = OrderStage.WaitingDestinationLocation;

            await bot.SendMessage(
                chatId: chatId,
                text: $"Qabul qilindi ✅\n📍 {state.PickupAddress}\n\n" +
                      "Endi *qayerga* borishni xohlaysiz?\n\n" +
                      "Boshqa nuqtani tanlash uchun:\n" +
                      "1️⃣ Xabar yozish qatori yonidagi 📎 (skrepka) belgisini bosing\n" +
                      "2️⃣ \"Location\" (Joylashuv) ni tanlang\n" +
                      "3️⃣ Xaritada kerakli nuqtani bosib, biroz ushlab turing\n" +
                      "4️⃣ \"Ushbu joyni yuborish\" tugmasini bosing",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
            return;
        }

        if (state.Stage == OrderStage.WaitingDestinationLocation)
        {
            state.DestinationLocation = GeoLocation.Create(location.Latitude, location.Longitude);
            state.DestinationAddress = await _geocodingService.GetAddressAsync(location.Latitude, location.Longitude);

            var customer = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.TelegramChatId == chatId);
            var mainMenu = BuildMainMenu();

            if (customer is null)
            {
                _userStateService.Reset(chatId);
                await bot.SendMessage(chatId, "❌ Xatolik: foydalanuvchi topilmadi. /start orqali qayta kiring.", replyMarkup: mainMenu, cancellationToken: cancellationToken);
                return;
            }

            var dto = new OrderForCreateDto
            {
                FromAddress = state.PickupAddress!,
                FromLatitude = state.PickupLocation!.Latitude,
                FromLongitude = state.PickupLocation.Longitude,

                ToAddress = state.DestinationAddress!,
                ToLatitude = state.DestinationLocation.Latitude,
                ToLongitude = state.DestinationLocation.Longitude,

                Source = "TelegramBot",
                PaymentMethod = "Cash"
            };

            var result = await _orderService.CreateAsync(customer.Id, dto);

            _userStateService.Reset(chatId);

            if (result.IsSuccess)
            {
                var order = result.Data!;
                await bot.SendMessage(
                    chatId: chatId,
                    text: $"✅ Buyurtma qabul qilindi!\n\n" +
                          $"📍 Qayerdan: {order.FromAddress}\n" +
                          $"🏁 Qayerga: {order.ToAddress}\n" +
                          $"📏 Masofa: {order.DistanceKm:F1} km\n" +
                          $"💰 Taxminiy narx: {order.EstimatedPrice:N0} so'm\n\n" +
                          $"Haydovchi tez orada tayinlanadi.",
                    replyMarkup: mainMenu,
                    cancellationToken: cancellationToken);
            }
            else
            {
                await bot.SendMessage(chatId, $"❌ {result.ErrorMessage}", replyMarkup: mainMenu, cancellationToken: cancellationToken);
            }
        }
    }

    private async Task HandleMyOrdersAsync(ITelegramBotClient bot, long chatId, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.TelegramChatId == chatId);

        if (customer is null)
        {
            await bot.SendMessage(chatId, "Avval tizimga kiring. /start ni yuboring.", cancellationToken: cancellationToken);
            return;
        }

        var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 5 };
        var result = await _orderService.GetByCustomerIdAsync(customer.Id, paginationParams);

        if (!result.IsSuccess || result.Data is null || result.Data.Items.Count == 0)
        {
            await bot.SendMessage(chatId, "Sizda hali buyurtmalar yo'q. \"🚕 Buyurtma berish\" orqali birinchi buyurtmangizni bering!", cancellationToken: cancellationToken);
            return;
        }

        var text = "📋 So'nggi buyurtmalaringiz:\n\n";

        foreach (var order in result.Data.Items)
        {
            text += $"🔸 {TranslateStatus(order.Status)}\n" +
                    $"📍 {order.FromAddress}\n" +
                    $"🏁 {order.ToAddress}\n" +
                    $"💰 {order.EstimatedPrice:N0} so'm | 🕒 {order.CreatedAt:dd.MM.yyyy HH:mm}\n\n";
        }

        await bot.SendMessage(chatId, text, cancellationToken: cancellationToken);
    }

    private static string TranslateStatus(string status) => status switch
    {
        "Pending" => "⏳ Kutilmoqda",
        "Accepted" => "✅ Qabul qilindi",
        "DriverArrived" => "🚗 Haydovchi yetib keldi",
        "InProgress" => "🛣️ Yo'lda",
        "Completed" => "🏁 Yakunlandi",
        "CancelledByCustomer" => "❌ Mijoz bekor qildi",
        "CancelledByDriver" => "❌ Haydovchi bekor qildi",
        _ => status
    };

    private static ReplyKeyboardMarkup BuildMainMenu()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "🚕 Buyurtma berish" },
            new KeyboardButton[] { "📋 Buyurtmalarim" },
        })
        {
            ResizeKeyboard = true
        };
    }

    private static ReplyKeyboardMarkup BuildLocationKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { KeyboardButton.WithRequestLocation("📍 Joylashuvni yuborish") }
        })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };
    }
}