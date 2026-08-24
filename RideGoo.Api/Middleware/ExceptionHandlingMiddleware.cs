using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RideGoo.Domain.Exceptions;
using RideGoo.Shared.Wrappers;

namespace RideGoo.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kutilmagan xatolik yuz berdi: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            DomainException => (HttpStatusCode.BadRequest, exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Ushbu amalni bajarishga ruxsatingiz yo'q."),
            KeyNotFoundException => (HttpStatusCode.NotFound, "So'ralgan ma'lumot topilmadi."),
            DbUpdateException => (HttpStatusCode.Conflict, "Ma'lumotlar bazasida xatolik yuz berdi."),
            _ => (HttpStatusCode.InternalServerError, "Serverda kutilmagan xatolik yuz berdi.")
        };

        var response = new ErrorResponse
        {
            Success = false,
            Message = message,
            StatusCode = (int)statusCode,
            Detail = _environment.IsDevelopment() ? exception.ToString() : null
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}