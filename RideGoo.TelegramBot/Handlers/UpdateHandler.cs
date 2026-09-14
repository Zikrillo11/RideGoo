using RideGoo.BLL.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RideGoo.TelegramBot.Handlers;

public class UpdateHandler
{
    private readonly IAuthService _authService;

    public UpdateHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message) return;

        var chatId = message.Chat.Id;

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
            var mainMenu = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "🚕 Buyurtma berish" },
                new KeyboardButton[] { "📋 Buyurtmalarim" },
            })
            {
                ResizeKeyboard = true
            };

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
}