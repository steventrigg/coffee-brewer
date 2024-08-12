using CoffeeBrewer.App.Coffee.Exceptions;
using CoffeeBrewer.App.Coffee.Queries;

namespace CoffeeBrewer.App.Coffee.Validators
{
    public class AprilFoolsValidator<T> : IValidator<BrewCoffeeQuery>
    {
        private readonly DateTime _now;

        public AprilFoolsValidator(DateTime? now = null)
        {
            // Assuming now local server time, as there is no date on the request
            _now = now ?? DateTime.Now;
        }

        public Task<Exception?> ValidateAsync(BrewCoffeeQuery request)
        {
            if (_now.Day == 1 && _now.Month == 4)
            {
                return Task.FromResult((Exception?)new BrewerIsATeapotException());
            }

            return Task.FromResult((Exception?)null);
        }
    }
}
