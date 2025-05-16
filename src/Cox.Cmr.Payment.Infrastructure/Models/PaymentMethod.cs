namespace Cox.Cmr.Payment.Infrastructure.Models;

[DynamoDBTable("payment-method")]
public class PaymentMethod
{
    [DynamoDBHashKey] public required string PaymentMethodId { get; set; }
    [DynamoDBRangeKey("SK")] public required string PaymentMethodSortKey { get; set; }
    public required string TenantId { get; set; }
    public required string OrganisationId { get; set; }
    public required BankAccount BankAccount { get; set; }
    public BillingDetails? BillingDetails { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public required string Type { get; set; }
}
