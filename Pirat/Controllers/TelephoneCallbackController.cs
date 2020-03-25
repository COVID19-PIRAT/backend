using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pirat.Model;
using Pirat.Services;

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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Post([FromBody] TelephoneCallbackRequest telephoneCallbackRequest)
        {
            this._mailService.sendTelephoneCallbackMail(telephoneCallbackRequest);
            return Ok();
        }

    }
}
