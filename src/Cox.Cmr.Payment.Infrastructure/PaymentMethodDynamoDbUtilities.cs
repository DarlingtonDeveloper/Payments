namespace Cox.Cmr.Payment.Infrastructure;

public class PaymentMethodDynamoDbUtilities
{
    internal const string PaymentMethodTableName = "payment-method";
    internal const string Sk = "Sk";
    internal const string PaymentMethodSkPrefix = "PaymentMethod#";
    internal const string OrganisationId = "OrganisationId";

    public static string GeneratePaymentSortKey() => PaymentMethodSkPrefix;
}
