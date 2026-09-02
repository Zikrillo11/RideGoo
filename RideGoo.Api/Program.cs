using RideGoo.Api.Configuration;
using RideGoo.Api.Filters;
using RideGoo.Api.Middleware;
using RideGoo.DAL.Data;
using RideGoo.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

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

// ...

var app = builder.Build();

app.UseGlobalExceptionHandling();

// Muhim: UseCorsConfiguration() — UseAuthentication()dan OLDIN turishi kerak
app.UseCorsConfiguration(); 

// ---------- Admin foydalanuvchini avtomatik yaratish ----------
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAdminAsync(dbContext);
}

// ---------- Middleware pipeline ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<RideHub>("/hubs/ride");

app.MapControllers();

app.Run();