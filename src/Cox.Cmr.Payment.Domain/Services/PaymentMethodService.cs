namespace Cox.Cmr.Payment.Domain.Services;

public class PaymentMethodService(
    ILogger<PaymentMethodService> logger,
    IPaymentMethodRepository paymentRepository,
    IPaymentMethodValidator paymentValidator,
    IDateTimeProvider dateTimeProvider
    ) : IPaymentMethodService
{
    public async Task<Models.PaymentMethod> Create(Models.PaymentMethod paymentMethodToCreate)
    {
        if (string.IsNullOrEmpty(paymentMethodToCreate.PaymentMethodId))
        {
            paymentMethodToCreate.PaymentMethodId = Guid.NewGuid().ToString();
        }

        var paymentValidationResult = await paymentValidator.ValidateAsync(paymentMethodToCreate);
        if (!paymentValidationResult.IsValid)
        {
            throw new ValidationException(paymentValidationResult.Errors);
        }

        paymentMethodToCreate.CreatedDateTime = dateTimeProvider.UtcNow;
        var createdPaymentMethod = await paymentRepository.Create(paymentMethodToCreate);
        using (LogContext.PushProperty("PaymentMethodId", paymentMethodToCreate.PaymentMethodId))
        {
            logger.LogInformation("Created new PaymentMethod");
        }

        return createdPaymentMethod;
    }
    public async Task<Models.PaymentMethod?> Get(string paymentMethodId)
       => await paymentRepository.GetByPaymentMethodId(paymentMethodId)
   ?? throw new NotFoundException($"PaymentMethod with PaymentMethodId {paymentMethodId} is not found");
}
