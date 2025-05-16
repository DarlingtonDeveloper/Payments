namespace Cox.Cmr.Payment.Infrastructure.Tests.Repositories;

public class PaymentMethodDynamoDbContainerFixture : IAsyncLifetime
{

    private AmazonDynamoDBClient? _amazonDynamoDbClient;
    private readonly DynamoDbContainer _dynamoDbContainer;
    private IDynamoDBContext _dynamoDbContext;

    public PaymentMethodDynamoDbContainerFixture() =>
    _dynamoDbContainer = new DynamoDbBuilder()
        .WithImage("amazon/dynamodb-local:latest")
        .Build();

    public IDynamoDBContext GetDynamoDbContext() => _dynamoDbContext;


    public async Task InitializeAsync()
    {
        await _dynamoDbContainer.StartAsync();

        var dynamoClientConfiguration = new AmazonDynamoDBConfig
        {
            ServiceURL = _dynamoDbContainer.GetConnectionString()
        };

        _amazonDynamoDbClient =
            new AmazonDynamoDBClient(new BasicAWSCredentials("accessKey", "secretKey"), dynamoClientConfiguration);

        _dynamoDbContext = new DynamoDBContext(_amazonDynamoDbClient, new DynamoDBContextConfig());

        var tables = await _amazonDynamoDbClient.ListTablesAsync();
        const string tableName = "payment-method";
        if (tables.TableNames.Contains(tableName))
        {
            await _amazonDynamoDbClient.DeleteTableAsync(new DeleteTableRequest(tableName));
        }

        //match table definition from /Terraform/infrastructure/dynamodb.tf
        await _amazonDynamoDbClient.CreateTableAsync(new CreateTableRequest
        {
            TableName = tableName,
            KeySchema =
            [
                new KeySchemaElement("PaymentMethodId", KeyType.HASH),
                new KeySchemaElement("SK", KeyType.RANGE)
            ],
            AttributeDefinitions =
            [
                new AttributeDefinition("PaymentMethodId", ScalarAttributeType.S),
                new AttributeDefinition("SK", ScalarAttributeType.S)
            ],
            ProvisionedThroughput = new ProvisionedThroughput(5, 5),
        });
    }

    public async Task CleanDynamoDbTable()
    {
        const string tableName = "payment-method";
        var scanRequest = new ScanRequest { TableName = tableName };

        var scanResponse = await _amazonDynamoDbClient!.ScanAsync(scanRequest);

        var deleteRequests = scanResponse.Items.Select(item => new WriteRequest
        {
            DeleteRequest = new DeleteRequest
            {
                Key = new Dictionary<string, AttributeValue>
            {
                { "PaymentMethodId", item["PaymentMethodId"] },
                { "SK", item["SK"] }
            }
            }
        }).ToList();

        foreach (var batch in deleteRequests.Chunk(25))
        {
            var batchWriteRequest = new BatchWriteItemRequest
            {
                RequestItems = new Dictionary<string, List<WriteRequest>> { { tableName, batch.ToList() } }
            };

            await _amazonDynamoDbClient.BatchWriteItemAsync(batchWriteRequest);
        }
    }
    public async Task DisposeAsync()
    {
        _dynamoDbContext?.Dispose();
        await _dynamoDbContainer.StopAsync();
        await _dynamoDbContainer.DisposeAsync();

        _amazonDynamoDbClient?.Dispose();
    }
}
