using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace RideGoo.TelegramBot.Services;

// Koordinatani (lat, lon) haqiqiy manzil nomiga aylantiradi (OpenStreetMap/Nominatim orqali)
public class GeocodingService
{
    private readonly HttpClient _httpClient;

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetAddressAsync(double latitude, double longitude)
    {
        try
        {
            var url = $"reverse?format=json&lat={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lon={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&accept-language=uz";

            var response = await _httpClient.GetFromJsonAsync<NominatimResponse>(url);

            return !string.IsNullOrWhiteSpace(response?.DisplayName)
                ? response.DisplayName
                : $"Joylashuv ({latitude:F5}, {longitude:F5})";
        }
        catch
        {
            // Internet yoki API muammosi bo'lsa, koordinataga qaytamiz (dastur to'xtamaydi)
            return $"Joylashuv ({latitude:F5}, {longitude:F5})";
        }
    }

    private class NominatimResponse
    {
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }
    }
}