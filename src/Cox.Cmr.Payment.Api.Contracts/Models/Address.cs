namespace Cox.Cmr.Payment.Api.Contracts.Models;

public record Address
{
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Town { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
}
