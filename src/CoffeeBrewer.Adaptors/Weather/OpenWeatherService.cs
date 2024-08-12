using System.Text.Json;

namespace CoffeeBrewer.Adaptors.Weather
{
    public class OpenWeatherService : IOpenWeatherService
    {
        private const string HOST = "https://api.openweathermap.org/data/2.5/weather";
        private const string KEY = "API_KEY";

        private readonly HttpClient _httpClient;

        public OpenWeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<double> GetCurrentTemperatureInCAsync(double lat, double lon, CancellationToken ctx)
        {
            // I would also add if I had time: tests, retry policy and caching

            var url = $"{HOST}?lat={lat}&lon={lon}&units=metric&appid={KEY}";

            var response = await _httpClient.GetAsync(url, ctx);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var weather = JsonSerializer.Deserialize<WeatherModel>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return weather?.Main.Temp
                ?? throw new Exception("Weather not found");
        }
    }
}
