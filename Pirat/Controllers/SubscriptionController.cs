using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pirat.Exceptions;
using Pirat.Extensions.Swagger.SwaggerConfiguration;
using Pirat.Model;
using Pirat.Other;
using Pirat.Services.Mail;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Controllers
{
    [ApiController]
    [Route("/subscription")]
    public class SubscriptionController: ControllerBase
    {
        private readonly IMailService _mailService;

        private readonly IMailInputValidatorService _mailInputValidatorService;

        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(IMailService mailService, IMailInputValidatorService mailInputValidatorService, ISubscriptionService subscriptionService)
        {
            _mailService = mailService;
            _mailInputValidatorService = mailInputValidatorService;
            _subscriptionService = subscriptionService;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerRequestExample(typeof(RegionSubscription), typeof(RegionSubscriptionExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        public IActionResult Post([FromBody] RegionSubscription regionsubscription)
        {
            NullCheck.ThrowIfNull<RegionSubscription>(regionsubscription);

            try
            {
                _mailInputValidatorService.validateMail(regionsubscription.email);
                this._subscriptionService.SubscribeRegionAsync(regionsubscription);
                this._mailService.SendRegionSubscriptionConformationMailAsync(regionsubscription);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (UnknownAdressException e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }
    }
}
