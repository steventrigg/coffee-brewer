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

        public Exception? Validate(BrewCoffeeQuery request)
        {
            var level = _repository.Get();

            if (level == 0)
            {
                _repository.Reset(4);

                return new BrewerEmptyException();
            }

            return null;
        }
    }
}
