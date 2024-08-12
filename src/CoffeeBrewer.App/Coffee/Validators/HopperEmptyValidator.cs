using CoffeeBrewer.Adaptors.Data;
using CoffeeBrewer.App.Coffee.Exceptions;
using CoffeeBrewer.App.Coffee.Queries;
using Microsoft.Extensions.Logging;

namespace CoffeeBrewer.App.Coffee.Validators
{
    public class HopperEmptyValidator<T> : IValidator<BrewCoffeeQuery>
    {
        private readonly IHopperLevelRepository _repository;
        private readonly ILogger<HopperEmptyValidator<BrewCoffeeQuery>> _logger;

        public HopperEmptyValidator(IHopperLevelRepository repository, ILogger<HopperEmptyValidator<BrewCoffeeQuery>> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Exception?> ValidateAsync(BrewCoffeeQuery request)
        {
            _logger.LogInformation("Validating hopper level!");

            // Having done this, I'd probably move it into the handler as a policy so validators don't need to be async
            var level = await _repository.GetAsync();

            if (level == 0)
            {
                _logger.LogInformation("Resetting hopper level!");

                await _repository.ResetAsync(4);

                return new BrewerEmptyException();
            }

            return null;
        }
    }
}
