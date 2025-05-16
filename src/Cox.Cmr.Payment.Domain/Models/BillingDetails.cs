namespace Cox.Cmr.Payment.Domain.Models;

public record BillingDetails
{
    public string? Name { get; set; }
    public Address? Address { get; set; }
}
