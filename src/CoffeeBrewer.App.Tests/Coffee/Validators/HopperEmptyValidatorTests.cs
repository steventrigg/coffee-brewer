using CoffeeBrewer.Adaptors.Data;
using CoffeeBrewer.App.Coffee.Exceptions;
using CoffeeBrewer.App.Coffee.Queries;
using CoffeeBrewer.App.Coffee.Validators;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoffeeBrewer.App.Tests.Coffee.Validators
{
    public class HopperEmptyValidatorTests
    {
        private readonly Mock<ILogger<HopperEmptyValidator<BrewCoffeeQuery>>> _mockLogger;

        public HopperEmptyValidatorTests()
        {
            _mockLogger = new Mock<ILogger<HopperEmptyValidator<BrewCoffeeQuery>>>();
        }

        [Fact]
        public async void Validator_When_Hopper_Level_Empty_Returns_BrewerEmptyException()
        {
            var mockHopperLevelRepository = new Mock<IHopperLevelRepository>();

            mockHopperLevelRepository.Setup(x => x.GetAsync())
                .ReturnsAsync(0);

            var sut = new HopperEmptyValidator<BrewCoffeeQuery>(mockHopperLevelRepository.Object, _mockLogger.Object);

            var exception = await sut.ValidateAsync(new BrewCoffeeQuery());

            Assert.IsType<BrewerEmptyException>(exception);
        }

        [Fact]
        public async void Validator_When_Hopper_Level_Empty_Resets_Hopper_Level()
        {
            const int defaultHopperLevel = 4;

            var mockHopperLevelRepository = new Mock<IHopperLevelRepository>();

            mockHopperLevelRepository.Setup(x => x.GetAsync())
                .ReturnsAsync(0);

            var sut = new HopperEmptyValidator<BrewCoffeeQuery>(mockHopperLevelRepository.Object, _mockLogger.Object);

            await sut.ValidateAsync(new BrewCoffeeQuery());

            mockHopperLevelRepository.Verify(x => x.ResetAsync(It.Is<int>(y => y == defaultHopperLevel)), Times.Twice);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async void Validator_When_Hopper_Level_Not_Empty_Returns_Null(int level)
        {
            var mockHopperLevelRepository = new Mock<IHopperLevelRepository>();

            mockHopperLevelRepository.Setup(x => x.GetAsync())
                .ReturnsAsync(level);

            var sut = new HopperEmptyValidator<BrewCoffeeQuery>(mockHopperLevelRepository.Object, _mockLogger.Object);

            var exception = await sut.ValidateAsync(new BrewCoffeeQuery());

            Assert.Null(exception);
        }
    }
}
