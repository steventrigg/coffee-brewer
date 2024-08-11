namespace CoffeeBrewer.App.Coffee.Models
{
    public class Brew
    {
        public string Message { get; } = "Your piping hot coffee is ready";
        public DateTimeOffset Prepared { get; } = DateTimeOffset.UtcNow;
    }
}
