using Cox.Cdp.Common.Providers;
using Microsoft.AspNetCore.Mvc;

namespace Cox.Cmr.Payment.Domain.Test.Service;

public class PaymentMethodServiceTests
{
    private readonly Fixture _fixture = new Fixture();
    private readonly IPaymentMethodValidator _paymentMethodValidator;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly ILogger<PaymentMethodService> _logger;   
    private readonly IPaymentMethodService _paymentMethodService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public PaymentMethodServiceTests()
    {
        _paymentMethodValidator = Substitute.For<IPaymentMethodValidator>();
        _paymentMethodRepository = Substitute.For<IPaymentMethodRepository>();
        _logger = Substitute.For<ILogger<PaymentMethodService>>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    _paymentMethodService = new PaymentMethodService(
            _logger,
            _paymentMethodRepository,
            _paymentMethodValidator,
            _dateTimeProvider
            );
    }

    [Fact]
    public async Task Create_WithValidPayment_ReturnsExpectedValue()
    {
        // Arrange
        var paymentMethodId = Guid.NewGuid();

        var currentDateTime = DateTime.UtcNow;

        var paymentToCreate = _fixture.Build<Domain.Models.PaymentMethod>()
            .With(x => x.PaymentMethodId, paymentMethodId.ToString())
            .Without(x => x.CreatedDateTime)            
            .Create();

        var createdPaymentMethod = paymentToCreate.DeepCopy();
        createdPaymentMethod!.CreatedDateTime = currentDateTime;

        _paymentMethodValidator.ValidateAsync(Arg.Any<Domain.Models.PaymentMethod>()).Returns(new ValidationResult());
        _paymentMethodRepository.Create(Arg.Any<Domain.Models.PaymentMethod>()).Returns(createdPaymentMethod);

        // Act
        var paymentResult = await _paymentMethodService.Create(paymentToCreate);

        // Assert
        paymentResult.ShouldNotBeNull();
        paymentResult.ShouldBeEquivalentTo(createdPaymentMethod);
    }

    [Fact]
    public async Task Get_PaymentMethodExists_ReturnsPaymentMethod()
    {
        var paymentMethodId = Guid.NewGuid().ToString();

        var paymentMethod = _fixture
            .Build<Models.PaymentMethod>()
            .With(x => x.PaymentMethodId, paymentMethodId)
            .Create();

        _paymentMethodRepository
            .GetByPaymentMethodId(paymentMethodId)
            .Returns(paymentMethod);

        // Act
        var contactResult = await _paymentMethodService.Get(paymentMethodId);

        // Assert
        contactResult.ShouldNotBeNull();
        contactResult.ShouldBeEquivalentTo(paymentMethod);
    }

    [Fact]
    public async Task Get_PaymentMethodDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var paymentMethodId = Guid.NewGuid().ToString();
        var currentDateTime = DateTime.UtcNow;

        // Act
        var createContact = async () => await _paymentMethodService.Get(paymentMethodId);      

        // Assert
        await createContact.ShouldThrowAsync<Domain.Exceptions.NotFoundException>();
           
    }

}
