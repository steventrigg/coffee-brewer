using Amazon.Runtime.Internal.Util;
using CoffeeBrewer.Adaptors.Data;
using CoffeeBrewer.App.Coffee.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CoffeeBrewer.App.Coffee.Queries
{
    public class BrewCoffeeQuery() : IRequest<Result<Brew>>;

    public class BrewCofeeQueryHandler : IRequestHandler<BrewCoffeeQuery, Result<Brew>>
    {
        private readonly IHopperLevelRepository _repository;
        private readonly ILogger<BrewCofeeQueryHandler> _logger;

        public BrewCofeeQueryHandler(IHopperLevelRepository repository, ILogger<BrewCofeeQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<Brew>> Handle(BrewCoffeeQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating a brew and decrementing hopper level.");

            await _repository.DecrementAsync();

            return new Result<Brew>(new Brew());
        }
    }
}
