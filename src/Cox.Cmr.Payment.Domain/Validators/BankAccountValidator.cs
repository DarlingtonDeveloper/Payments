namespace Cox.Cmr.Payment.Domain.Validators;

public class BankAccountValidator : AbstractValidator<BankAccount>
{
    public BankAccountValidator()
    {
        RuleFor(bankAccount => bankAccount.BankName)
            .NotEmpty()
            .WithMessage($"'{nameof(Models.BankAccount.BankName)}' must not be empty.");

        RuleFor(bankAccount => bankAccount.AccountName)
           .NotEmpty()
           .WithMessage($"'{nameof(Models.BankAccount.AccountName)}' must not be empty.");

        RuleFor(bankAccount => bankAccount.AccountNumber)
          .NotEmpty()
          .WithMessage($"'{nameof(Models.BankAccount.AccountNumber)}' must not be empty.")
          .Must(x => x?.Length == 8)         
          .Matches("^\\d{8}")
          .WithMessage($"'{nameof(Models.BankAccount.AccountNumber)}' must have exactly 8 digits.");

        RuleFor(bankAccount => bankAccount.SortCode)
         .NotEmpty()
         .WithMessage($"'{nameof(Models.BankAccount.SortCode)}' must not be empty.")
         .Must(x => x?.Length == 6)
         .Matches("^\\d{6}")
         .WithMessage($"'{nameof(Models.BankAccount.SortCode)}' must have exactly 6 digits.");
    }
}
