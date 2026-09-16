using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RideGoo.BLL.Interfaces;
using RideGoo.BLL.Services;
using RideGoo.DAL.Data;
using RideGoo.DAL.Repositories;
using RideGoo.Domain.Interfaces;
using RideGoo.TelegramBot;
using RideGoo.TelegramBot.Handlers;
using RideGoo.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

// ---------- Database ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------- Unit of Work + Services ----------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<INotificationHub, NullNotificationHub>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddSingleton<UserStateService>();

// ---------- AutoMapper ----------
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<RideGoo.BLL.Mappings.MappingProfile>());

// ---------- Telegram Bot Client ----------
var botToken = builder.Configuration["TelegramBot:Token"]
    ?? throw new InvalidOperationException("Telegram bot tokeni topilmadi.");

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));

var app = builder.Build();

var botClient = app.Services.GetRequiredService<ITelegramBotClient>();
var me = await botClient.GetMe();
Console.WriteLine($"Bot ishga tushdi: @{me.Username}");

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

botClient.StartReceiving(
    updateHandler: async (bot, update, token) =>
    {
        using var scope = app.Services.CreateScope();
        var handler = new UpdateHandler(
    scope.ServiceProvider.GetRequiredService<IAuthService>(),
    scope.ServiceProvider.GetRequiredService<IOrderService>(),
    scope.ServiceProvider.GetRequiredService<IUnitOfWork>(),
    scope.ServiceProvider.GetRequiredService<UserStateService>());

        await handler.HandleUpdateAsync(bot, update, token);
    },
    errorHandler: (bot, exception, token) =>
    {
        Console.WriteLine($"Xatolik: {exception.Message}");
        return Task.CompletedTask;
    },
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

Console.WriteLine("Bot xabarlarni kutmoqda... Toxtatish uchun Ctrl+C bosing.");

await app.RunAsync();