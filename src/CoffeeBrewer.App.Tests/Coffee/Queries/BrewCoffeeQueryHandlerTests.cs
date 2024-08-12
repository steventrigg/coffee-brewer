using CoffeeBrewer.Adaptors.Data;
using CoffeeBrewer.App.Coffee.Models;
using CoffeeBrewer.App.Coffee.Policies;
using CoffeeBrewer.App.Coffee.Queries;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoffeeBrewer.App.Tests.Coffee.Validators
{
    public class BrewCoffeeQueryHandlerTests
    {
        private readonly Mock<ILogger<BrewCofeeQueryHandler>> _mockLogger;

        public BrewCoffeeQueryHandlerTests()
        {
            _mockLogger = new Mock<ILogger<BrewCofeeQueryHandler>>();
        }

        [Fact]
        public async Task Handler_Returns_Result_With_Brew()
        {
            var mockHopperLevelRepository = new Mock<IHopperLevelRepository>();

            var sut = new BrewCofeeQueryHandler(mockHopperLevelRepository.Object, null, _mockLogger.Object);

            var result = await sut.Handle(new BrewCoffeeQuery(), default);

            Assert.NotNull(result);
            Assert.IsType<Brew>(result.Value);
        }

        [Fact]
        public async Task Handler_Decrements_Hopper_Level()
        {
            var mockHopperLevelRepository = new Mock<IHopperLevelRepository>();

            var sut = new BrewCofeeQueryHandler(mockHopperLevelRepository.Object, null, _mockLogger.Object);

            await sut.Handle(new BrewCoffeeQuery(), default);

            mockHopperLevelRepository.Verify(x => x.DecrementAsync(), Times.Once);
        }

        [Fact]
        public async Task Handler_Applies_Temperature_Policy()
        {
            var mockHopperLevelRepository = new Mock<IHopperLevelRepository>();
            var mockTemperaturePolicy = new Mock<ITempPolicy<Brew>>();

            var sut = new BrewCofeeQueryHandler(mockHopperLevelRepository.Object, mockTemperaturePolicy.Object, _mockLogger.Object);

            var result = await sut.Handle(new BrewCoffeeQuery(), default);

            mockTemperaturePolicy.Verify(x => x.ApplyPolicyAsync(It.IsAny<Brew>(), default), Times.Once);
        }
    }
}