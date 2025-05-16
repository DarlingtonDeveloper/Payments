namespace Cox.Cmr.Payment.Infrastructure.Repositories.Search;

public interface IPaymentMethodSearchRepository
{
    public Task<Models.PaymentMethod?> GetPaymentMethodById(string paymentMethodId);
}
