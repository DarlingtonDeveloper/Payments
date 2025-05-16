using Asp.Versioning;
using AutoMapper;
using Cox.Cmr.Payment.Api.Attributes;
using Cox.Cmr.Payment.Api.Contracts.Request;
using Cox.Cmr.Payment.Api.Contracts.Responses;
using Cox.Cmr.Payment.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Cox.Cmr.Payment.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("payment/v{version:apiVersion}/payment-method")]
public class PaymentMethodController(
    IPaymentMethodService paymentMethodService,
    IMapper mapper
    ) : ControllerBase
{

    [HttpPost]
    [Produces("application/json")]
    [Authorize(Policy = PaymentApiPolicies.Write)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [IncludeInDocumentation]
    public async Task<ActionResult<CreatePaymentMethodResponse>> Post(CreatePaymentMethodRequest request)
    {
        var createRequest = mapper.Map<Domain.Models.PaymentMethod>(request);

        var paymentMethodResult = await paymentMethodService.Create(createRequest);
        var paymentMethodResponse = mapper.Map<CreatePaymentMethodResponse>(paymentMethodResult);

        return new CreatedResult($"{Request.GetDisplayUrl()}/{paymentMethodResponse.PaymentMethodId}", paymentMethodResponse);
    }

    [HttpGet]
    [Authorize(Policy = PaymentApiPolicies.Read)]
    [Route("{paymentMethodId:guid}")]
    [IncludeInDocumentation]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<Contracts.Models.PaymentMethod>> GetPaymentMethodById(Guid paymentMethodId)
    {
        var paymentMethod = await paymentMethodService.Get(paymentMethodId.ToString());
        var response = mapper.Map<Contracts.Models.PaymentMethod>(paymentMethod);
        return Ok(response);
    }
}
