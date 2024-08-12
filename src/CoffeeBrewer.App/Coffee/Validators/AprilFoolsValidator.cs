using CoffeeBrewer.App.Coffee.Exceptions;
using CoffeeBrewer.App.Coffee.Queries;
using Microsoft.Extensions.Logging;

namespace CoffeeBrewer.App.Coffee.Validators
{
    public class AprilFoolsValidator<T> : IValidator<BrewCoffeeQuery>
    {
        private readonly DateTime _now;
        private readonly ILogger<AprilFoolsValidator<BrewCoffeeQuery>> _logger;

        public AprilFoolsValidator(ILogger<AprilFoolsValidator<BrewCoffeeQuery>> logger, DateTime? now = null)
        {
            // Assuming now local server time, as there is no date on the request
            _now = now ?? DateTime.Now;

            _logger = logger;
        }

        public Task<Exception?> ValidateAsync(BrewCoffeeQuery request)
        {
            _logger.LogInformation("Validating april fools!");

            if (_now.Day == 1 && _now.Month == 4)
            {
                return Task.FromResult((Exception?)new BrewerIsATeapotException());
            }

            return Task.FromResult((Exception?)null);
        }
    }
}
