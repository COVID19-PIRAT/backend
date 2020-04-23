using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pirat.Exceptions;
using Pirat.Extensions.Swagger.SwaggerConfiguration;
using Pirat.Model;
using Pirat.Other;
using Pirat.Services;
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
        
        private readonly IConfigurationService _configurationService;

        public SubscriptionController(
            IMailService mailService,
            IMailInputValidatorService mailInputValidatorService, 
            ISubscriptionService subscriptionService,
            IConfigurationService configurationService)
        {
            _mailService = mailService;
            _mailInputValidatorService = mailInputValidatorService;
            _subscriptionService = subscriptionService;
            _configurationService = configurationService;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerRequestExample(typeof(RegionSubscription), typeof(RegionSubscriptionExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        public IActionResult Post([FromQuery] [Required] string region, [FromBody] RegionSubscription regionsubscription)
        {
            NullCheck.ThrowIfNull<RegionSubscription>(regionsubscription);

            try
            {
                _configurationService.ThrowIfUnknownRegion(region);
                _mailInputValidatorService.validateMail(regionsubscription.email);
                this._subscriptionService.SubscribeRegionAsync(regionsubscription, region);
                this._mailService.SendRegionSubscriptionConformationMailAsync(region, regionsubscription);
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
