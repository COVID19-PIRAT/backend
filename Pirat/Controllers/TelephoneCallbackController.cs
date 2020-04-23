using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    [Route("/telephone-callback")]
    public class TelephoneCallbackController : ControllerBase
    {

        private readonly IMailService _mailService;
        private readonly IMailInputValidatorService _mailInputValidatorService;
        private readonly IConfigurationService _configurationService;


        public TelephoneCallbackController(
            IMailService mailService, 
            IMailInputValidatorService mailInputValidatorService,
            IConfigurationService configurationService)
        {
            _mailService = mailService;
            _mailInputValidatorService = mailInputValidatorService;
            _configurationService = configurationService;
        }


        [HttpPost]
        [SwaggerRequestExample(typeof(TelephoneCallbackRequest), typeof(TelephoneCallbackRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        public IActionResult Post([FromQuery] [Required] string region, [FromBody] TelephoneCallbackRequest telephoneCallbackRequest)
        {
            NullCheck.ThrowIfNull<TelephoneCallbackRequest>(telephoneCallbackRequest);

            try
            {
                _configurationService.ThrowIfUnknownRegion(region);
                _mailInputValidatorService.validateMail(telephoneCallbackRequest.email);
                this._mailService.SendTelephoneCallbackMailAsync(region, telephoneCallbackRequest);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
