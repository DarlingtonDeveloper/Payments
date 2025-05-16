namespace Cox.Cmr.Payment.Infrastructure.Models;

public class BankAccount
{
    public required string BankName { get; set; }
    public required string AccountName { get; set; }
    public required string AccountNumber { get; set; }
    public required string SortCode { get; set; }
}
