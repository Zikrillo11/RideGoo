using RideGoo.Api.Configuration;
using RideGoo.Api.Filters;
using RideGoo.Api.Middleware;
using RideGoo.DAL.Data;

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

var app = builder.Build();

// ---------- Global Exception Handling — eng boshida ----------
app.UseGlobalExceptionHandling();

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

app.MapControllers();

app.Run();