namespace Cox.Cmr.Payment.Domain.Models;

public record BankAccount
{
    public required string BankName { get; set; }
    public required string AccountName { get; set; }
    public required string AccountNumber { get; set; }
    public required string SortCode { get; set; }
}
