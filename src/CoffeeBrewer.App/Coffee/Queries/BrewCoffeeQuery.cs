using CoffeeBrewer.Adaptors.Data;
using CoffeeBrewer.App.Coffee.Models;
using CoffeeBrewer.App.Coffee.Policies;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeBrewer.App.Coffee.Queries
{
    public class BrewCoffeeQuery() : IRequest<Result<Brew>>;

    public class BrewCofeeQueryHandler : IRequestHandler<BrewCoffeeQuery, Result<Brew>>
    {
        private readonly IHopperLevelRepository _repository;
        private readonly ITempPolicy<Brew> _tempPolicy;
        private readonly ILogger<BrewCofeeQueryHandler> _logger;

        public BrewCofeeQueryHandler(IHopperLevelRepository repository, ITempPolicy<Brew> tempPolicy, ILogger<BrewCofeeQueryHandler> logger)
        {
            _repository = repository;
            _tempPolicy = tempPolicy;
            _logger = logger;
        }

        public async Task<Result<Brew>> Handle(BrewCoffeeQuery request, CancellationToken ctx)
        {
            _logger.LogInformation("Creating a brew and decrementing hopper level.");

            await _repository.DecrementAsync();

            var brew = new Brew();
            
            // I need a better way to apply these
            if (_tempPolicy != null)
            {
                brew = await _tempPolicy.ApplyPolicyAsync(brew, ctx);
            }

            return new Result<Brew>(brew);
        }
    }
}
