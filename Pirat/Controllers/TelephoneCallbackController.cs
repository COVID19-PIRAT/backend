using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pirat.Codes;
using Pirat.Extensions.Swagger.SwaggerConfiguration;
using Pirat.Model;
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

        public TelephoneCallbackController(IMailService mailService)
        {
            _mailService = mailService;
        }


        [HttpPost]
        [SwaggerRequestExample(typeof(TelephoneCallbackRequest), typeof(TelephoneCallbackRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        public IActionResult Post([FromBody] TelephoneCallbackRequest telephoneCallbackRequest)
        {
            if (!_mailService.verifyMail(telephoneCallbackRequest.email))
            {
                return BadRequest(Error.ErrorCodes.INVALID_MAIL);
            }
            this._mailService.sendTelephoneCallbackMail(telephoneCallbackRequest);
            return Ok();
        }

    }
}
