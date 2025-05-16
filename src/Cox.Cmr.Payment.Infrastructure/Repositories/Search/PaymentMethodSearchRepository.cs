namespace Cox.Cmr.Payment.Infrastructure.Repositories.Search;

public class PaymentMethodSearchRepository(
    PrincipalContext principalContext,
    IDynamoDBContext dynamoDbContext)
    : BasePaymentMethodSearchRepository, IPaymentMethodSearchRepository
{
    public async Task<Models.PaymentMethod?> GetPaymentMethodById(string paymentMethodId)
    {
        var organisationFilter = OrganisationIdFilter();

        var operationConfig = new DynamoDBOperationConfig { QueryFilter = [organisationFilter] };

        var queryResult = await dynamoDbContext.QueryAsync<Models.PaymentMethod?>(
                paymentMethodId, QueryOperator.Equal, [PaymentMethodDynamoDbUtilities.GeneratePaymentSortKey()], operationConfig)
            .GetRemainingAsync();

        var paymentMethod = queryResult.FirstOrDefault();

        return paymentMethod;
    }

    private ScanCondition OrganisationIdFilter() => new(
        PaymentMethodDynamoDbUtilities.OrganisationId,
        ScanOperator.In,
        principalContext.OrganisationIds?.ToArray<object>());
}
