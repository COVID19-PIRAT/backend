using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pirat.Codes;
using Pirat.Extensions.Swagger.SwaggerConfiguration;
using Pirat.Model;
using Pirat.Model.Api.Resource;
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


        public TelephoneCallbackController(IMailService mailService, IMailInputValidatorService mailInputValidatorService)
        {
            _mailService = mailService;
            _mailInputValidatorService = mailInputValidatorService;
        }


        [HttpPost]
        [SwaggerRequestExample(typeof(TelephoneCallbackRequest), typeof(TelephoneCallbackRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        public IActionResult Post([FromBody] TelephoneCallbackRequest telephoneCallbackRequest)
        {
            NullCheck.ThrowIfNull<TelephoneCallbackRequest>(telephoneCallbackRequest);

            try
            {
                _mailInputValidatorService.validateMail(telephoneCallbackRequest.email);
                this._mailService.SendTelephoneCallbackMailAsync(telephoneCallbackRequest);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
