using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;

namespace CoffeeBrewer.Adaptors.Data
{
    public class DynDbHopperLevelRepository : IHopperLevelRepository
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly string _tableName;

        private const string KEY = "Key";
        private const string KEY_VALUE = "HopperLevel";
        private const string LEVEL = "Level";

        public DynDbHopperLevelRepository(IAmazonDynamoDB dynamoDbClient, string tableName, ILogger<DynDbHopperLevelRepository> logger)
        {
            _dynamoDbClient = dynamoDbClient;
            _tableName = tableName;

            if (string.IsNullOrEmpty(tableName))
            {
                logger.LogError("Hopper level table name is empty.");
            }
        }

        public async Task<int> GetAsync()
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
            if (response.Item != null && response.Item.ContainsKey(LEVEL))
            {
                return int.Parse(response.Item[LEVEL].N);
            }

            return default;
        }

        public async Task DecrementAsync()
        {
            // Ideally this method would be atomic

            var level = await GetAsync();

            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { KEY, new AttributeValue { S = KEY_VALUE } },
                    { LEVEL, new AttributeValue { N = (level - 1).ToString() } }
                }
            };

            await _dynamoDbClient.PutItemAsync(request);
        }

        public async Task ResetAsync(int level)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { KEY, new AttributeValue { S = KEY_VALUE } },
                    { LEVEL, new AttributeValue { N = level.ToString() } }
                }
            };

            await _dynamoDbClient.PutItemAsync(request);
        }
    }
}
