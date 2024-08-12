using CoffeeBrewer.Adaptors.Weather;
using CoffeeBrewer.App.Coffee.Exceptions;
using CoffeeBrewer.App.Coffee.Models;
using CoffeeBrewer.App.Coffee.Policies;
using Moq;

namespace CoffeeBrewer.App.Tests.Coffee.Policies
{
    public class TempPolicyTests
    {
        [Theory]
        [InlineData(31)]
        [InlineData(100)]
        public async void Policy_Above_Temperature_Applies_Ice_Tea(double temperature)
        {
            const string expectedMessge = "Your refreshing iced coffee is ready";

            var mockWeatherService = new Mock<IOpenWeatherService>();

            mockWeatherService.Setup(x => x.GetCurrentTemperatureInCAsync(It.IsAny<double>(), It.IsAny<double>(), default))
                .ReturnsAsync(temperature);

            var sut = new TempPolicy<Brew>(mockWeatherService.Object);

            var result = await sut.ApplyPolicyAsync(new Brew(), default);

            Assert.Equal(expectedMessge, result.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(30)]
        public async void Policy_Below_Temperature_Does_Not_Apply(double temperature)
        {
            var brew = new Brew();
            var expectedMessage = brew.Message;

            var mockWeatherService = new Mock<IOpenWeatherService>();

            mockWeatherService.Setup(x => x.GetCurrentTemperatureInCAsync(It.IsAny<double>(), It.IsAny<double>(), default))
                .ReturnsAsync(temperature);

            var sut = new TempPolicy<Brew>(mockWeatherService.Object);

            var result = await sut.ApplyPolicyAsync(brew, default);

            Assert.Equal(expectedMessage, result.Message);
        }
    }
}
