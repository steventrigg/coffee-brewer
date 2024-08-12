namespace CoffeeBrewer.Adaptors.Weather
{
    public class WeatherModel
    {
        public required Main Main { get; init; }
    }

    public class Main
    {
        public double Temp { get; init; }
    }
}
