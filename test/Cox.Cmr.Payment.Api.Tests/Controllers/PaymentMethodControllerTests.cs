namespace Cox.Cmr.Payment.Api.Tests.Controllers;

public class PaymentMethodControllerTests
{
    private readonly IPaymentMethodService _paymentMethodService;
    private readonly IMapper _mapper;
    private readonly IFixture _fixture;
    private readonly HttpContext _httpContext;
    private readonly PaymentMethodController _controller;

    public PaymentMethodControllerTests()
    {
        var mapperConfiguration = new MapperConfiguration(configuration
            => configuration.AddProfile(new PaymentMethodApiProfile()));

        _mapper = mapperConfiguration.CreateMapper();
        _paymentMethodService = Substitute.For<IPaymentMethodService>();
        _fixture = new Fixture();
        _httpContext = Substitute.For<HttpContext>();

        _controller = new PaymentMethodController(_paymentMethodService, _mapper)
        {
            ControllerContext = new ControllerContext { HttpContext = _httpContext }
        };
    }

    [Fact]
    public async Task Post_WithValidRequest_ReturnsCreatedResponse()
    {
        // Arrange
        var currentDateTime = DateTime.UtcNow;

        var createPaymentMethodRequest = _fixture.Create<CreatePaymentMethodRequest>();

        var createdPaymentMethod = new Domain.Models.PaymentMethod()
        {
            TenantId = createPaymentMethodRequest.TenantId,
            OrganisationId = createPaymentMethodRequest.OrganisationId,
            CreatedDateTime = currentDateTime,
            Type = createPaymentMethodRequest.Type,
            BankAccount = new Domain.Models.BankAccount
            {
                AccountName = createPaymentMethodRequest.BankAccount.AccountName,
                AccountNumber = createPaymentMethodRequest.BankAccount.AccountNumber,
                BankName = createPaymentMethodRequest.BankAccount.BankName,
                SortCode = createPaymentMethodRequest.BankAccount.SortCode
            },
            BillingDetails = new Domain.Models.BillingDetails
            {
                Name = createPaymentMethodRequest.BillingDetails.Name,
                Address = new Domain.Models.Address
                {
                    AddressLine1 = createPaymentMethodRequest.BillingDetails.Address?.AddressLine1,
                    AddressLine2 = createPaymentMethodRequest.BillingDetails.Address?.AddressLine2,
                    Town = createPaymentMethodRequest.BillingDetails.Address?.Town,
                    Country = createPaymentMethodRequest.BillingDetails.Address!.Country,
                    PostalCode = createPaymentMethodRequest.BillingDetails.Address!.PostalCode
                }
            }
        };

        _paymentMethodService
            .Create(Arg.Any<Domain.Models.PaymentMethod>())
            .Returns(Task.FromResult(createdPaymentMethod));

        var substituteRequest = Substitute.For<HttpRequest>();
        var substituteContext = Substitute.For<HttpContext>();
        substituteContext.Request.Returns(substituteRequest);

        _controller.ControllerContext = new ControllerContext { HttpContext = substituteContext };

        // Act
        var response = await _controller.Post(createPaymentMethodRequest);

        // Assert
        response.Result.ShouldNotBeNull();
        response.Result.ShouldBeOfType(typeof(CreatedResult));
        var createdResult = (CreatedResult)response.Result!;
        createdResult.Value.ShouldBeOfType(typeof(CreatePaymentMethodResponse));
    }

    [Fact]
    public async Task GetById_PaymentExist_ReturnsOk()
    {
        // Arrange
        var paymentMethodId = Guid.NewGuid();
        var paymentMethod = _fixture.Build<Domain.Models.PaymentMethod>()
            .With(x => x.PaymentMethodId, paymentMethodId.ToString())
            .Create();

        _paymentMethodService.Get(paymentMethodId.ToString()).Returns(paymentMethod);

        // Act
        var apiResponse = await _controller.GetPaymentMethodById(paymentMethodId);

        // Assert
        apiResponse.ShouldNotBeNull();
        apiResponse.Result.ShouldBeOfType<OkObjectResult>();
        var okObjectResult = (OkObjectResult)apiResponse.Result!;
        var objectResponse = okObjectResult.Value as Contracts.Models.PaymentMethod;
        objectResponse.ShouldNotBeNull();
        objectResponse.PaymentMethodId.ShouldBe(paymentMethod.PaymentMethodId);
 
    }
}
