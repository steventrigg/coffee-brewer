namespace CoffeeBrewer.Adaptors.Weather
{
    public interface IOpenWeatherService
    {
        public Task<double> GetCurrentTemperatureInCAsync(double lat, double lon, CancellationToken ctx);
    }
}
