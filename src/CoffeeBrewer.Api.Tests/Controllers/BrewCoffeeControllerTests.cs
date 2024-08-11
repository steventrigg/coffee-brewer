using CoffeeBrewer.Api.Controllers;
using CoffeeBrewer.App;
using CoffeeBrewer.App.Coffee.Exceptions;
using CoffeeBrewer.App.Coffee.Models;
using CoffeeBrewer.App.Coffee.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace CoffeeBrewer.Api.Tests.Controllers
{
    public class BrewCoffeeControllerTests
    {
        [Fact]
        public async Task BrewCoffee_Returns_Success()
        {
            var mockMediator = new Mock<IMediator>();

            mockMediator.Setup(x => x.Send(It.IsAny<BrewCoffeeQuery>(), default))
                .ReturnsAsync(new Result<Brew>(new Brew()));

            var sut = new BrewCoffeeController(mockMediator.Object);

            var result = await sut.BrewCoffee(default) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task BrewCoffee_Returns_Brew_With_ISO8601_Date_Format()
        {
            const string isoFormat = "yyyy-MM-ddTHH:mm:ss.fffffffK";
            const string dateRegex = "(?<=\\\"Prepared\\\":\\\")(.*?)(?=\\\")";

            var mockMediator = new Mock<IMediator>();

            mockMediator.Setup(x => x.Send(It.IsAny<BrewCoffeeQuery>(), default))
                .ReturnsAsync(new Result<Brew>(new Brew()));

            var sut = new BrewCoffeeController(mockMediator.Object);

            var result = await sut.BrewCoffee(default) as OkObjectResult;
            Assert.NotNull(result);

            var json = JsonConvert.SerializeObject(result.Value);
            string dateValue = Regex.Match(json, dateRegex).Value;

            Assert.True(DateTime.TryParseExact(dateValue, isoFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
        }

        [Fact]
        public async Task BrewCoffee_On_BrewerEmptyException_Returns_ServiceUnavailable()
        {
            var mockMediator = new Mock<IMediator>();

            mockMediator.Setup(x => x.Send(It.IsAny<BrewCoffeeQuery>(), default))
                .ReturnsAsync(new Result<Brew>(new BrewerEmptyException()));

            var sut = new BrewCoffeeController(mockMediator.Object);

            var result = await sut.BrewCoffee(default) as StatusCodeResult;

            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.ServiceUnavailable, result.StatusCode);
        }

        [Fact]
        public async Task BrewCoffee_On_BrewerIsATeapotException_Returns_ImATeapot()
        {
            const int imATeapotStatusCode = 418; // Because Microsoft didn't inlcude it in HttpStatCode :'(

            var mockMediator = new Mock<IMediator>();

            mockMediator.Setup(x => x.Send(It.IsAny<BrewCoffeeQuery>(), default))
                .ReturnsAsync(new Result<Brew>(new BrewerIsATeapotException()));

            var sut = new BrewCoffeeController(mockMediator.Object);

            var result = await sut.BrewCoffee(default) as StatusCodeResult;

            Assert.NotNull(result);
            Assert.Equal(imATeapotStatusCode, result.StatusCode);
        }
    }
}
