using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;

namespace CoffeeBrewer.Adaptors.Data
{
    public class DynDbHopperLevelRepository : IHopperLevelRepository
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly string _tableName;
        private readonly ILogger<DynDbHopperLevelRepository> _logger;

        private const string KEY = "key";
        private const string KEY_VALUE = "level";
        private const string PROP_NAME = "level";

        public DynDbHopperLevelRepository(IAmazonDynamoDB dynamoDbClient, string tableName, ILogger<DynDbHopperLevelRepository> logger)
        {
            _dynamoDbClient = dynamoDbClient;
            _tableName = tableName;
            _logger = logger;

            if (string.IsNullOrEmpty(tableName))
            {
                _logger.LogError("Hopper level table name is empty.");
            }
        }

        public async Task<int> GetAsync()
        {
            _logger.LogInformation("Getting hopper level.");

            try
            {
                var request = new GetItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                {
                    { KEY, new AttributeValue { S = KEY_VALUE } }
                }
                };

                var response = await _dynamoDbClient.GetItemAsync(request);

                if (response.Item != null && response.Item.ContainsKey(PROP_NAME))
                {
                    return int.Parse(response.Item[PROP_NAME].N);
                }
            } catch (Exception ex)
            {
                _logger.LogError("Dynamodb read error.", ex.Message);
            }

            return default;
        }

        public async Task DecrementAsync()
        {
            // Ideally this method would be atomic
            _logger.LogInformation("Decrementing hopper level.");

            var level = await GetAsync();

            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { KEY, new AttributeValue(KEY_VALUE) },
                    { PROP_NAME, new AttributeValue((level - 1).ToString()) }
                }
            };

            await _dynamoDbClient.PutItemAsync(request);
        }

        public async Task ResetAsync(int level)
        {
            _logger.LogInformation("Resetting hopper level.");

            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { KEY, new AttributeValue(KEY_VALUE) },
                    { PROP_NAME, new AttributeValue(level.ToString()) }
                }
            };

            await _dynamoDbClient.PutItemAsync(request);
        }
    }
}
