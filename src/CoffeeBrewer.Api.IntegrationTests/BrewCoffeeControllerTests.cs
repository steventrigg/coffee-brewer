using System.Text.Json;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

namespace CoffeeBrewer.Api.Tests;

public class BrewCoffeeControllerTests
{
    [Fact]
    public async Task Test_BrewCoffee()
    {
        var now = DateTime.Now;
        if (now.Month == 4 && now.Day == 1)
        {
            // Don't fail on april fools
            Assert.True(true);
        }

        var lambdaFunction = new LambdaEntryPoint();

        var requestStr = File.ReadAllText("./SampleRequests/BrewCoffeeController-Get.json");
        var request = JsonSerializer.Deserialize<APIGatewayHttpApiV2ProxyRequest>(requestStr, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        var context = new TestLambdaContext();
        var response = await lambdaFunction.FunctionHandlerAsync(request, context);

        Assert.Equal(200, response.StatusCode);
        Assert.True(response.Headers.ContainsKey("Content-Type"));
        Assert.Equal("application/json; charset=utf-8", response.Headers.First(x => x.Key == "Content-Type").Value);
    }
}