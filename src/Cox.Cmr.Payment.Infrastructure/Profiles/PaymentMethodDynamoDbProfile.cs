namespace Cox.Cmr.Payment.Infrastructure.Profiles;

public  class PaymentMethodDynamoDbProfile : Profile
{
    public PaymentMethodDynamoDbProfile()
    {

        CreateMap<Domain.Models.PaymentMethod, Infrastructure.Models.PaymentMethod>();
        CreateMap<Domain.Models.BillingDetails, Infrastructure.Models.BillingDetails>();
        CreateMap<Domain.Models.BankAccount, Infrastructure.Models.BankAccount>();
        CreateMap<Domain.Models.Address, Infrastructure.Models.Address>()
               .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode.ToUpper()));
        CreateMap<Domain.Models.Address, Infrastructure.Models.Address>();

        CreateMap<Infrastructure.Models.PaymentMethod, Domain.Models.PaymentMethod>();
        CreateMap<Infrastructure.Models.Address, Domain.Models.Address>();
        CreateMap<Infrastructure.Models.BillingDetails, Domain.Models.BillingDetails>();
        CreateMap<Infrastructure.Models.BankAccount, Domain.Models.BankAccount>();
    }
}
