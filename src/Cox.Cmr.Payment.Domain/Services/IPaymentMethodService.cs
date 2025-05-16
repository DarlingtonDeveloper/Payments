namespace Cox.Cmr.Payment.Domain.Services;

public interface IPaymentMethodService
{
    public Task<Models.PaymentMethod> Create(Models.PaymentMethod paymentMethod);
    public Task<Models.PaymentMethod?> Get(string paymentMethodId);
}
