namespace CoffeeBrewer.Adaptors.Data
{
    public interface IHopperLevelRepository
    {
        public Task<int> GetAsync();

        public Task DecrementAsync();

        public Task ResetAsync(int level);
    }
}
