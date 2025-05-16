namespace Cox.Cmr.Payment.Api.Contracts.Models;

public record BillingDetails
{
    public string? Name { get; set; }
    public Address? Address { get; set; }
}
