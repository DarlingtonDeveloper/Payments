namespace Cox.Cmr.Payment.Infrastructure.Tests.Repositories;

public class PaymentMethodRepositoryTests:IClassFixture<PaymentMethodDynamoDbContainerFixture>
{
    private readonly DateTime _utcNow;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;
    private readonly IPaymentMethodRepository _paymentRepository;         
    private readonly IDynamoDBContext _dynamoDbContext;
    private readonly PaymentMethodDynamoDbContainerFixture _paymentDynamoDbContainerFixture;

    private const string TenantId = "997e5f9d-5c2b-40e6-828b-c96369c1e854";
    private const string OrganisationId1 = "43954320-9a1f-4192-8b39-022f7ccd585a";      

    public PaymentMethodRepositoryTests(PaymentMethodDynamoDbContainerFixture paymentDynamoDbContainerFixture)
    {
        _paymentDynamoDbContainerFixture = paymentDynamoDbContainerFixture;
        _dynamoDbContext = paymentDynamoDbContainerFixture.GetDynamoDbContext();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _dateTimeProvider.UtcNow.Returns(_utcNow);

        var logger = NSubstitute.Substitute.For<ILogger<PaymentMethodRepository>>();

        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile(new PaymentMethodDynamoDbProfile());
        });
        _mapper = mapperConfiguration.CreateMapper();

        _paymentRepository = new PaymentMethodRepository(_dynamoDbContext, _mapper, _dateTimeProvider);


    }

    [Fact]
    public async Task Create_WithValidDomainModel_CreatePaymentMethod()
    {
        //Arrange
        var fixture = new Fixture();

        var paymentMethodId = Guid.NewGuid().ToString();
        var address = fixture.Build<Domain.Models.Address>()
                 .With(p => p.PostalCode, "A1A 1A1")
                 .Create();

        var billingDetails = fixture.Build<Domain.Models.BillingDetails>()
                .With(b => b.Address, address)
                .Create();

        var bankAccount = fixture.Build<Domain.Models.BankAccount>()
              .With(b => b.AccountName, "Account Name")
              .With(b => b.AccountNumber, "2222420000001113")
              .With(b => b.BankName, "Bank Name")              
              .With(b => b.SortCode, "000001")
              .Create();

        var paymentToCreate = fixture.Build<Domain.Models.PaymentMethod>()
            .With(x => x.PaymentMethodId, paymentMethodId)
            .With(x => x.TenantId, TenantId)
            .With(x => x.OrganisationId, OrganisationId1)
            .With(x => x.BankAccount, bankAccount)
            .With(x => x.BillingDetails, billingDetails)
            .With(x => x.Type, "BankAccount")
            .Create();

        // Act
        var returnedPaymentMethod = await _paymentRepository.Create(paymentToCreate);
        var createdPaymentMethod = await _dynamoDbContext.LoadAsync<Models.PaymentMethod>(paymentToCreate.PaymentMethodId,
            PaymentMethodDynamoDbUtilities.GeneratePaymentSortKey());

        //Assert
        createdPaymentMethod.ShouldNotBeNull();
        createdPaymentMethod.PaymentMethodId.ShouldBe(paymentMethodId);
        createdPaymentMethod.TenantId.ShouldBe(TenantId);
        createdPaymentMethod.OrganisationId.ShouldBe(OrganisationId1);
        createdPaymentMethod.BankAccount.ShouldNotBeNull();
        returnedPaymentMethod.BankAccount.AccountName.ShouldBe("Account Name");
        createdPaymentMethod.BankAccount.AccountName.ShouldBe("Account Name");
        returnedPaymentMethod.BankAccount.AccountNumber.ShouldBe("2222420000001113");
        createdPaymentMethod.BankAccount.AccountNumber.ShouldBe("2222420000001113");
        returnedPaymentMethod.BankAccount.BankName.ShouldBe("Bank Name");
        createdPaymentMethod.BankAccount.BankName.ShouldBe("Bank Name");

        returnedPaymentMethod.Type.ShouldBe("BankAccount");
        createdPaymentMethod.Type.ShouldBe("BankAccount");

        createdPaymentMethod.BillingDetails.ShouldNotBeNull();
        createdPaymentMethod.BillingDetails.Address.ShouldNotBeNull();            
        createdPaymentMethod.BillingDetails.Address.PostalCode.ShouldNotBeNull();
        createdPaymentMethod.BillingDetails.Address.Country.ShouldNotBeNull();
        createdPaymentMethod.BillingDetails.Address.PostalCode.ShouldBe("A1A 1A1");
        returnedPaymentMethod.BillingDetails.Address.Country.ShouldNotBeNull();
    }
}
