using CoffeeBrewer.Adaptors.Weather;
using CoffeeBrewer.App.Coffee.Models;

namespace CoffeeBrewer.App.Coffee.Policies
{
    public interface ITempPolicy<T> : IPolicy<Brew>;

    public class TempPolicy<T> : ITempPolicy<Brew>
    {
        private readonly IOpenWeatherService _weatherService;

        private const double LAT = -36.787717517368804;
        private const double LON = 174.85805363942922;

        private const int ICE_TEA_TEMP = 30;

        public TempPolicy(IOpenWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task<Brew> ApplyPolicyAsync(Brew model, CancellationToken ctx)
        {
            var currentTemp = await _weatherService.GetCurrentTemperatureInCAsync(LAT, LON, ctx);

            if (currentTemp > ICE_TEA_TEMP)
            {
                return new Brew
                {
                    Message = "Your refreshing iced coffee is ready",
                    Prepared = model.Prepared
                };
            }

            return model;
        }
    }
}
