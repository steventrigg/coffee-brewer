using CoffeeBrewer.App.Coffee.Exceptions;
using CoffeeBrewer.App.Coffee.Queries;
using CoffeeBrewer.App.Coffee.Validators;

namespace CoffeeBrewer.App.Tests.Coffee.Validators
{
    public class AprilFoolsValidatorTests
    {
        [Theory]
        [InlineData(1990, 4, 1)]
        [InlineData(2024, 4, 1)]
        [InlineData(2025, 4, 1)]
        public async void Validator_On_AprilFoods_Returns_BrewerIsATeapotException(int year, int month, int day)
        {
            var aprilFoolsDate = new DateTime(year, month, day);

            var sut = new AprilFoolsValidator<object>(aprilFoolsDate);

            var exception = await sut.ValidateAsync(new BrewCoffeeQuery());

            Assert.IsType<BrewerIsATeapotException>(exception);
        }

        [Theory]
        [InlineData(2024, 3, 31)]
        [InlineData(2024, 4, 2)]
        public async void Validator_Not_AprilFoods_Returns_Null(int year, int month, int day)
        {
            var notAprilFoolsDate = new DateTime(year, month, day);

            var sut = new AprilFoolsValidator<object>(notAprilFoolsDate);

            var exception = await sut.ValidateAsync(new BrewCoffeeQuery());

            Assert.Null(exception);
        }
    }
}