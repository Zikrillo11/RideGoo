using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RideGoo.BLL.Interfaces;
using RideGoo.BLL.Services;
using RideGoo.DAL.Data;
using RideGoo.DAL.Repositories;
using RideGoo.Domain.Interfaces;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

// ---------- Database ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------- Unit of Work + Services (backend bilan bir xil) ----------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IDriverService, DriverService>();

// ---------- AutoMapper ----------
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<RideGoo.BLL.Mappings.MappingProfile>());

// ---------- Telegram Bot Client ----------
var botToken = builder.Configuration["TelegramBot:Token"]
    ?? throw new InvalidOperationException("Telegram bot tokeni topilmadi.");

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));

var app = builder.Build();

// ---------- Botni ishga tushirish ----------
var botClient = app.Services.GetRequiredService<ITelegramBotClient>();
var me = await botClient.GetMe();
Console.WriteLine($"Bot ishga tushdi: @{me.Username}");

app.Run();