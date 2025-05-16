namespace Cox.Cmr.Payment.Api.Contracts.Models;

public record PaymentMethod
{
    public string? PaymentMethodId { get; set; }
    public required string TenantId { get; set; }
    public required string OrganisationId { get; set; }
    public required BankAccount BankAccount { get; set; }
    public BillingDetails? BillingDetails { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public required string Type { get; set; }
}
