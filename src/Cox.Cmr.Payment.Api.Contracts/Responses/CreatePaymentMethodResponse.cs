using Cox.Cmr.Payment.Api.Contracts.Models;

namespace Cox.Cmr.Payment.Api.Contracts.Responses;

public record CreatePaymentMethodResponse
{
    public required string PaymentMethodId { get; set; }
    public required string TenantId { get; set; }
    public required string OrganisationId { get; set; }
    public required BankAccount BankAccount { get; set; }
    public required BillingDetails BillingDetails { get; set; }
    public required DateTime CreatedDateTime { get; set; }
    public required string Type { get; set; }
};
