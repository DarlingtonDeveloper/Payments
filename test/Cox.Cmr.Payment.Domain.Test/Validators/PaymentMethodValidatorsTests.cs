namespace Cox.Cmr.Payment.Domain.Test.Validators;

public class PaymentMethodValidatorsTests
{
    private readonly PaymentMethodValidator _validator;
    private Domain.Models.PaymentMethod _paymentMethod;

    public PaymentMethodValidatorsTests()
    {
        _validator = new();
        _paymentMethod = new Domain.Models.PaymentMethod
        {
            PaymentMethodId = Guid.NewGuid().ToString(),
            TenantId = Guid.NewGuid().ToString(),
            OrganisationId = Guid.NewGuid().ToString(),
            CreatedDateTime = DateTime.Now,
            Type = "BankAccount",
            BillingDetails = new Models.BillingDetails
            {
                Name = "Billing Name",
                Address = new Models.Address
                {
                    Country = "Country",
                    PostalCode = "MDG SF4",
                }
            },
            BankAccount = new Models.BankAccount
            {
                AccountName = "AccountName",
                AccountNumber = "31926819",
                BankName = "Bank Name",
                SortCode = "601613"

            }
        };
    }

    [Fact]
    public void Validate_ValidPaymentMethod_IsValidTrue()
    {
        var paymentMethod = _paymentMethod;

        var validationResult = _validator.Validate(paymentMethod);

        validationResult.IsValid.ShouldBeTrue();
    }

    [InlineData("qwerty")]
    [InlineData("")]
    [InlineData(null)]
    [Theory]
    public void Validate_PaymentMethodIdInvalid_ValidationFail(string? value)
    {
        var invalidPaymentMethod = _paymentMethod with { PaymentMethodId = value };

        var validationResult = _validator.Validate(invalidPaymentMethod);

        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.ShouldContain(e => e.PropertyName == "PaymentMethodId");
    }

    [InlineData("qwerty")]
    [InlineData("")]
    [InlineData(null)]
    [Theory]
    public void Validate_TenantIdInvalid_ValidationFail(string? value)
    {
        var invalidPaymentMethod = _paymentMethod with { TenantId = value };

        var validationResult = _validator.Validate(invalidPaymentMethod);

        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.ShouldContain(e => e.PropertyName == "TenantId");
    }

    [InlineData("qwerty")]
    [InlineData("")]
    [InlineData(null)]
    [Theory]
    public void Validate_OrganisationIdInvalid_ValidationFail(string? value)
    {
        var invalidPaymentMethod = _paymentMethod with { OrganisationId = value };

        var validationResult = _validator.Validate(invalidPaymentMethod);

        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.ShouldContain(e => e.PropertyName == "OrganisationId");
    }

    [InlineData("")]
    [InlineData(null)]
    [Theory]
    public void Validate_InvalidType_Fails(string? value)
    {
        var invalidPaymentMethod = _paymentMethod with { Type = value };

        var validationResult = _validator.Validate(invalidPaymentMethod);

        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.ShouldContain(e => e.PropertyName == "Type");
    }

    [Fact]
    public void Validate_AbsentBankAccount_Fails()
    {
        var invalidPaymentMethod = _paymentMethod with { BankAccount = null };

        var validationResult = _validator.Validate(invalidPaymentMethod);

        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.ShouldContain(e => e.PropertyName == "BankAccount");
    }

    [Fact]
    public void Validate_AbsentBankName_Fails()
    {
        var invalidPaymentMethod = _paymentMethod with
        {
            BankAccount = _paymentMethod.BankAccount with { BankName = null }
        };

        var validationResult = _validator.Validate(invalidPaymentMethod);

        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.ShouldContain(e => e.PropertyName == "BankAccount.BankName");
    }

    [Fact]
    public void Validate_AbsentAccountName_Fails()
    {
        var invalidPaymentMethod = _paymentMethod with
        {
            BankAccount = _paymentMethod.BankAccount with { AccountName = null}
        };

        var validationResult = _validator.Validate(invalidPaymentMethod);

        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.ShouldContain(e => e.PropertyName == "BankAccount.AccountName");
    }

    [Fact]
    public void Validate_AbsentBillingDetailsAddress_Success()
    {
        var invalidPaymentMethod = _paymentMethod with
        {
            BillingDetails = _paymentMethod.BillingDetails with { Address = null }
        };

        var validationResult = _validator.Validate(invalidPaymentMethod);

        validationResult.IsValid.ShouldBeTrue();
        validationResult.Errors.Count.ShouldBe(0);
    }

    [Fact]
    public void Validate_AbsentBillingDetails_Success()
    {
        var invalidPaymentMethod = _paymentMethod with
        {
            BillingDetails = _paymentMethod.BillingDetails with { Address = null }
        };

        var validationResult = _validator.Validate(invalidPaymentMethod);

        validationResult.IsValid.ShouldBeTrue();
        validationResult.Errors.Count.ShouldBe(0);
    }

    [InlineData("123456789")]
    [InlineData("123456ds")]
    [InlineData("")]
    [InlineData(null)]
    [Theory]
    public void Validate_AbsentAccountNumber_Fails(string? value)
    {
        var invalidPaymentMethod = _paymentMethod with
        {
            BankAccount = _paymentMethod.BankAccount with { AccountNumber = value }
        };

        var validationResult = _validator.Validate(invalidPaymentMethod);

        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.ShouldContain(e => e.PropertyName == "BankAccount.AccountNumber");
    }

    [InlineData("12")]
    [InlineData("1s23")]
    [InlineData("1234")]
    [InlineData("ababab")]
    [InlineData("123456s")]
    [InlineData("")]
    [InlineData(null)]
    [Theory]
    public void Validate_AbsentSortCode_Fails(string? value)
    {
        var invalidPaymentMethod = _paymentMethod with
        {
            BankAccount = _paymentMethod.BankAccount with { SortCode = value }
        };

        var validationResult = _validator.Validate(invalidPaymentMethod);

        validationResult.IsValid.ShouldBeFalse();
        validationResult.Errors.ShouldContain(e => e.PropertyName == "BankAccount.SortCode");
    }
}
