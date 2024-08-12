namespace CoffeeBrewer.App.Coffee.Models
{
    public class Brew
    {
        public string Message { get; init; } = "Your piping hot coffee is ready";
        public DateTimeOffset Prepared { get; init; } = DateTimeOffset.UtcNow;
    }
}
