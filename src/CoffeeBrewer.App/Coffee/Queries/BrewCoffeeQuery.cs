using CoffeeBrewer.Adaptors.Data;
using CoffeeBrewer.App.Coffee.Models;
using MediatR;

namespace CoffeeBrewer.App.Coffee.Queries
{
    public class BrewCoffeeQuery() : IRequest<Result<Brew>>;

    public class BrewCofeeQueryHandler : IRequestHandler<BrewCoffeeQuery, Result<Brew>>
    {
        private readonly IHopperLevelRepository _repository;

        public BrewCofeeQueryHandler(IHopperLevelRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Brew>> Handle(BrewCoffeeQuery request, CancellationToken cancellationToken)
        {
            await _repository.DecrementAsync();

            return new Result<Brew>(new Brew());
        }
    }
}
