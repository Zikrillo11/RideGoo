using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using RideGoo.Api.Configuration;
using RideGoo.Api.Filters;
using RideGoo.Api.Hubs;
using RideGoo.Api.Middleware;
using RideGoo.DAL.Data;
using Serilog;
using Serilog.Events;

// ---------- Serilog'ni eng boshida sozlaymiz ----------
// (bu "bootstrap logger" — hali WebApplicationBuilder yaratilmasdan oldingi xatolarni ham ushlab qoladi)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateLogger();

try
{
    Log.Information("RideGoo.Api ishga tushmoqda...");

    var builder = WebApplication.CreateBuilder(args);

    // Endi ASP.NET Core'ning o'zi ham Serilog orqali log yozadi
    builder.Host.UseSerilog();

    // ---------- Configuration modullari ----------
    builder.Services.AddDatabaseConfiguration(builder.Configuration);
    builder.Services.AddApplicationServicesConfiguration();
    builder.Services.AddJwtAuthenticationConfiguration(builder.Configuration);
    builder.Services.AddSwaggerConfiguration();

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    });

    builder.Services.AddSignalR();

    builder.Services.AddCorsConfiguration();

    // ---------- Rate limiting ----------
    builder.Services.AddRateLimiter(options =>
    {
        options.AddFixedWindowLimiter("AuthLimiter", opt =>
        {
            opt.PermitLimit = 5;
            opt.Window = TimeSpan.FromMinutes(1);
            opt.QueueLimit = 0;
        });

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });

    var app = builder.Build();

    app.UseGlobalExceptionHandling();

    // Har bir HTTP so'rovni (endpoint, status kod, davomiylik) avtomatik log qiladi
    app.UseSerilogRequestLogging();

    app.UseCorsConfiguration();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
        await DbSeeder.SeedAdminAsync(dbContext, app.Configuration);
    }

    // ---------- Middleware pipeline ----------
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseRateLimiter();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHub<RideHub>("/hubs/ride");

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "RideGoo.Api kutilmagan xatolik bilan to'xtadi");
}
finally
{
    Log.CloseAndFlush();
}