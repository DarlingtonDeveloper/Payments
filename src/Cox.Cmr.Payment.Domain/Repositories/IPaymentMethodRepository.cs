
namespace Cox.Cmr.Payment.Domain.Repositories;

public interface IPaymentMethodRepository
{
    public Task<Models.PaymentMethod> Create(Models.PaymentMethod paymentMethod);
    public Task<Models.PaymentMethod?> GetByPaymentMethodId(string paymentMethodId);
}
