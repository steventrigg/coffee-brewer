using CoffeeBrewer.Adaptors.Data;
using CoffeeBrewer.App.Coffee.Exceptions;
using CoffeeBrewer.App.Coffee.Queries;

namespace CoffeeBrewer.App.Coffee.Validators
{
    public class HopperEmptyValidator<T> : IValidator<BrewCoffeeQuery>
    {
        private readonly IHopperLevelRepository _repository;

        public HopperEmptyValidator(IHopperLevelRepository repository)
        {
            _repository = repository;
        }

        public async Task<Exception?> ValidateAsync(BrewCoffeeQuery request)
        {
            // Having done this, I'd probably move it into the handler as a policy so validators don't need to be async
            var level = await _repository.GetAsync();

            if (level == 0)
            {
                await _repository.ResetAsync(4);

                return new BrewerEmptyException();
            }

            return null;
        }
    }
}
