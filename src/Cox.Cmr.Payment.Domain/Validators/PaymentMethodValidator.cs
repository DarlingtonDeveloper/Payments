namespace Cox.Cmr.Payment.Domain.Validators;
public class PaymentMethodValidator : AbstractValidator<Models.PaymentMethod>, IPaymentMethodValidator
{
    public PaymentMethodValidator()
    {
        RuleFor(paymentMethod => paymentMethod.PaymentMethodId)
            .NotEmpty().WithMessage($"'{nameof(Models.PaymentMethod.PaymentMethodId)}' must not be empty.")
            .Must(ValidationHelper.IsValidGuid).WithMessage($"'{nameof(Models.PaymentMethod.PaymentMethodId)}' must be a guid.");         

        RuleFor(paymentMethod => paymentMethod.TenantId)
            .NotEmpty().WithMessage($"'{nameof(Models.PaymentMethod.TenantId)}' must not be empty.")
            .Must(ValidationHelper.IsValidGuid).WithMessage($"'{nameof(Models.PaymentMethod.TenantId)}' must be a guid.");

        RuleFor(paymentMethod => paymentMethod.OrganisationId)
            .NotEmpty()
            .WithMessage($"'{nameof(Models.PaymentMethod.OrganisationId)}' must not be empty.")
            .Must(ValidationHelper.IsValidGuid)
            .WithMessage($"'{nameof(Models.PaymentMethod.OrganisationId)}' must be a guid.");

        RuleFor(paymentMethod => paymentMethod.Type)
            .NotEmpty()
            .WithMessage($"'{nameof(Models.PaymentMethod.Type)}' must not be empty.")
            .Equal("BankAccount", StringComparer.OrdinalIgnoreCase)
            .WithMessage($"'{nameof(Models.PaymentMethod.Type)}' must be equal with 'BankAccount'.");  
       

        RuleFor(paymentMethod => paymentMethod .BankAccount)
            .NotNull()
            .SetValidator(new BankAccountValidator());
    }
}


