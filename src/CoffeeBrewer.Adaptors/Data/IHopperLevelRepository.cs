namespace CoffeeBrewer.Adaptors.Data
{
    public interface IHopperLevelRepository
    {
        public int Get();

        public void Decrement();

        public void Reset(int level);
    }
}
