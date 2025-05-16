namespace Cox.Cmr.Payment.Infrastructure.Repositories;

public class PaymentMethodRepository(
    IDynamoDBContext dynamoDbContext,
    IMapper mapper, 
    IDateTimeProvider dateTimeProvider) : IPaymentMethodRepository
{
    public async Task<Domain.Models.PaymentMethod> Create(Domain.Models.PaymentMethod payment)
    {       
        var currentDateTime = dateTimeProvider.UtcNow;
        var paymentMethodEntity = mapper.Map<Models.PaymentMethod>(payment);

        paymentMethodEntity.PaymentMethodSortKey = PaymentMethodDynamoDbUtilities.GeneratePaymentSortKey();
        paymentMethodEntity.CreatedDateTime = currentDateTime;

        await dynamoDbContext.SaveAsync(paymentMethodEntity);

        var domainPaymentMethod = mapper.Map<Domain.Models.PaymentMethod>(paymentMethodEntity);
        return domainPaymentMethod;
    }
    public async Task<Domain.Models.PaymentMethod?> GetByPaymentMethodId(string paymentMethodId)
    {
        var queryResult = await dynamoDbContext.QueryAsync<Infrastructure.Models.PaymentMethod>(
          paymentMethodId, QueryOperator.Equal, [PaymentMethodDynamoDbUtilities.PaymentMethodSkPrefix])
                .GetRemainingAsync();

        var paymentMethod = queryResult.FirstOrDefault();

        if (paymentMethod is null)
        {
            return null;
        }

        var domainPaymentMethod = mapper.Map<Domain.Models.PaymentMethod>(paymentMethod);
        return domainPaymentMethod;
    }
}
