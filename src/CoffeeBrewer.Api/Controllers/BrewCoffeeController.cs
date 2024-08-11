using CoffeeBrewer.App.Coffee.Exceptions;
using CoffeeBrewer.App.Coffee.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CoffeeBrewer.Api.Controllers;

[Route("/")]
public class BrewCoffeeController : ControllerBase
{
    protected readonly IMediator Mediator;

    public BrewCoffeeController(IMediator mediator)
    {
        Mediator = mediator;
    }

    [HttpGet("brew-coffee")]
    public async Task<IActionResult> BrewCoffee(CancellationToken ctx)
    {
        var result = await Mediator.Send(new BrewCoffeeQuery(), ctx);

        if (result.HasError && result.Exception is BrewerEmptyException)
        {
            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        if (result.HasError && result.Exception is BrewerIsATeapotException)
        {
            return StatusCode(418);
        }

        return Ok(result.Value);
    }
}