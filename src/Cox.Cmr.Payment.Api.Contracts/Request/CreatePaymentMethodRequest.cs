namespace Cox.Cmr.Payment.Api.Contracts.Request;

public record CreatePaymentMethodRequest(
    string TenantId,
    string OrganisationId,
    BankAccount BankAccount,
    BillingDetails? BillingDetails,   
    string Type
);
