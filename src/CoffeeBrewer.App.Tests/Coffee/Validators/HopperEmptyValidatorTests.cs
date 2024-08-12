using CoffeeBrewer.Adaptors.Data;
using CoffeeBrewer.App.Coffee.Exceptions;
using CoffeeBrewer.App.Coffee.Queries;
using CoffeeBrewer.App.Coffee.Validators;
using Moq;

namespace CoffeeBrewer.App.Tests.Coffee.Validators
{
    public class HopperEmptyValidatorTests
    {
        [Fact]
        public async void Validator_When_Hopper_Level_Empty_Returns_BrewerEmptyException()
        {
            var mockHopperLevelRepository = new Mock<IHopperLevelRepository>();

            mockHopperLevelRepository.Setup(x => x.GetAsync())
                .ReturnsAsync(0);

            var sut = new HopperEmptyValidator<BrewCoffeeQuery>(mockHopperLevelRepository.Object);

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

            var sut = new HopperEmptyValidator<BrewCoffeeQuery>(mockHopperLevelRepository.Object);

            await sut.ValidateAsync(new BrewCoffeeQuery());

            mockHopperLevelRepository.Verify(x => x.ResetAsync(It.Is<int>(y => y == defaultHopperLevel)), Times.Once);
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

            var sut = new HopperEmptyValidator<BrewCoffeeQuery>(mockHopperLevelRepository.Object);

            var exception = await sut.ValidateAsync(new BrewCoffeeQuery());

            Assert.Null(exception);
        }
    }
}
