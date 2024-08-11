namespace CoffeeBrewer.Adaptors.Data
{
    public class LocalHopperLevelRepository : IHopperLevelRepository
    {
        private const int Default = 4;
        private static int? _hopperLevel = 4;

        public int Get()
        {
            if (!_hopperLevel.HasValue)
            {
                _hopperLevel = Default;
            }

            return _hopperLevel.Value;
        }

        public void Decrement()
        {
            _hopperLevel--;
        }

        public void Reset(int level = Default)
        {
            _hopperLevel = level;
        }
    }
}
