using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;

namespace CoffeeBrewer.Adaptors.Data
{
    public class LocalHopperLevelRepository : IHopperLevelRepository
    {
        private const int Default = 4;
        private static int? _hopperLevel = 4;

        public Task<int> GetAsync()
        {
            if (!_hopperLevel.HasValue)
            {
                _hopperLevel = Default;
            }

            return Task.FromResult(_hopperLevel.Value);
        }

        public Task DecrementAsync()
        {
            _hopperLevel--;

            return Task.CompletedTask;
        }

        public Task ResetAsync(int level = Default)
        {
            _hopperLevel = level;

            return Task.CompletedTask;
        }
    }
}
