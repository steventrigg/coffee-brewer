using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using CoffeeBrewer.Adaptors.Data;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoffeeBrewer.Adaptors.Tests
{
    public class DynDbHopperRepositoryTests
    {
        private readonly Mock<ILogger<DynDbHopperLevelRepository>> _mockLogger;

        public DynDbHopperRepositoryTests()
        {
            _mockLogger = new Mock<ILogger<DynDbHopperLevelRepository>>();
        }

        [Fact]
        public async void Get_Level_Returns_Level_Number()
        {
            const string tableName = "hopperleveltable";
            const int expectedLevel = 4;

            var mockDynamoDbClient = new Mock<IAmazonDynamoDB>();

            var mockResponse = new GetItemResponse
            {
                Item = new Dictionary<string, AttributeValue>
                {
                    { "key", new AttributeValue { S = "level" } },
                    { "level", new AttributeValue { N = expectedLevel.ToString() } }
                }
            };

            mockDynamoDbClient.Setup(x => 
                x.GetItemAsync(It.Is<GetItemRequest>(y => y.TableName == tableName), default))
                    .ReturnsAsync(mockResponse);

            var sut = new DynDbHopperLevelRepository(mockDynamoDbClient.Object, tableName, _mockLogger.Object);

            var result = await sut.GetAsync();

            Assert.Equal(expectedLevel, result);
        }

        [Fact]
        public async void Decrements_Level_Decreases_Level_Be_One()
        {
            const string tableName = "hopperlevel";
            const int originalLevel = 3;
            const int expectedLevel = originalLevel - 1;

            var mockDynamoDbClient = new Mock<IAmazonDynamoDB>();

            var mockResponse = new GetItemResponse
            {
                Item = new Dictionary<string, AttributeValue>
                {
                    { "key", new AttributeValue { S = "level" } },
                    { "level", new AttributeValue { N = originalLevel.ToString() } }
                }
            };

            mockDynamoDbClient.Setup(x =>
                x.GetItemAsync(It.Is<GetItemRequest>(y => y.TableName == tableName), default))
                    .ReturnsAsync(mockResponse);

            var sut = new DynDbHopperLevelRepository(mockDynamoDbClient.Object, tableName, _mockLogger.Object);

            await sut.DecrementAsync();

            mockDynamoDbClient.Verify(x => x.PutItemAsync(It.Is<PutItemRequest>(y => y.Item["level"].N == expectedLevel.ToString()), default), Times.Once);
        }

        [Fact]
        public async void Reset_Level_Sets_To_Original_Level()
        {
            const string tableName = "hopperlevel";
            const int originalLevel = 4;

            var mockDynamoDbClient = new Mock<IAmazonDynamoDB>();

            var mockResponse = new GetItemResponse
            {
                Item = new Dictionary<string, AttributeValue>
                {
                    { "key", new AttributeValue { S = "level" } },
                    { "level", new AttributeValue { N = originalLevel.ToString() } }
                }
            };

            mockDynamoDbClient.Setup(x =>
                x.GetItemAsync(It.Is<GetItemRequest>(y => y.TableName == tableName), default))
                    .ReturnsAsync(mockResponse);

            var sut = new DynDbHopperLevelRepository(mockDynamoDbClient.Object, tableName, _mockLogger.Object);

            await sut.ResetAsync(originalLevel);

            mockDynamoDbClient.Verify(x => x.PutItemAsync(It.Is<PutItemRequest>(y => y.Item["level"].N == originalLevel.ToString()), default), Times.Once);
        }
    }
}