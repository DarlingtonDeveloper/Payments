using AutoMapper;
using Cox.Cdp.Common.Providers;
using Cox.Cmr.Payment.Api.Contracts.Request;
using Cox.Cmr.Payment.Api.Contracts.Responses;

namespace Cox.Cmr.Payment.Api.Profiles
{
    public class PaymentMethodApiProfile : Profile
    {
        public PaymentMethodApiProfile()
        {      
            CreateMap<Domain.Models.PaymentMethod, Contracts.Models.PaymentMethod>();
            CreateMap<Contracts.Models.PaymentMethod, Domain.Models.PaymentMethod>();

            CreateMap<CreatePaymentMethodRequest, Domain.Models.PaymentMethod>()
                .ForMember(x => x.PaymentMethodId, opt => opt.Ignore());         
       
            CreateMap<Domain.Models.PaymentMethod, CreatePaymentMethodResponse>()
                .ForMember(dest => dest.CreatedDateTime,
                src => src.MapFrom(opt => ToDateTimeString(opt.CreatedDateTime)));

            CreateMap<Domain.Models.Address, Contracts.Models.Address>();
            CreateMap<Contracts.Models.Address, Domain.Models.Address>();

            CreateMap<Domain.Models.BillingDetails, Contracts.Models.BillingDetails>();
            CreateMap<Contracts.Models.BillingDetails, Domain.Models.BillingDetails>();

            CreateMap<Domain.Models.BankAccount, Contracts.Models.BankAccount>();
            CreateMap<Contracts.Models.BankAccount, Domain.Models.BankAccount>();
        }

        private static string ToDateTimeString(DateTime dateTime) =>
    dateTime.ToString(IDateTimeProvider.CdpDateFormatString);
    }
}
