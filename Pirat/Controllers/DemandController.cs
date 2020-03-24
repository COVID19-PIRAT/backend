using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pirat.DatabaseContext;
using Pirat.Exceptions;
using Pirat.Model;
using Pirat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Controllers
{

    [ApiController]
    [Route("/resources")]
    public class DemandController : ControllerBase
    {

        private readonly ILogger<DemandController> _logger;

        private readonly IDemandService _demandService;

        private readonly IMailService _mailService;

        private readonly IReCaptchaService _reCaptchaService;

        public DemandController(
            ILogger<DemandController> logger,
            IDemandService demandService,
            IMailService mailService,
            IReCaptchaService reCaptchaService
            )
        {
            _logger = logger;
            _demandService = demandService;
            _mailService = mailService;
            _reCaptchaService = reCaptchaService;
        }

        //***********GET REQUESTS


        [HttpGet("consumables")]
        [ProducesResponseType(typeof(List<OfferItem<Consumable>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromQuery] Consumable consumable, [FromQuery] Address address)
        {
            try
            {
                consumable.address = address;
                return Ok(await _demandService.QueryOffers(consumable));
            } catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpGet("devices")]
        [ProducesResponseType(typeof(List<OfferItem<Device>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromQuery] Device device, [FromQuery] Address address)
        {
            try
            {
                device.address = address;
                return Ok(await _demandService.QueryOffers(device));
            } catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("manpower")]
        [ProducesResponseType(typeof(List<OfferItem<Personal>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromQuery] Manpower manpower)
        {
            try
            {
                return Ok(await _demandService.QueryOffers(manpower));
            } catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
           
        }

        [HttpGet("offers/{token}")]
        [ProducesResponseType(typeof(Aggregate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Get(string token)
        {
            try
            {
                return Ok(await _demandService.queryLink(token));
            } catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

        }

        //*********POST REQUESTS


        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody] ReCaptchaWrapper<Offer> body)
        {
            string reCaptchaResponse = body.recaptchaResponse;
            bool isValidRequest = await this._reCaptchaService.ValidateResponse(reCaptchaResponse);
            if (!isValidRequest)
            {
                // TODO Maaaaaaaaaaxxxxx, Hilfe? Sinnvolle Antwort / Status code und sooo...
                throw new Exception("No bots!");
            }

            Offer offer = body.inner;
            try
            {
                if (!_mailService.verifyMail(offer.provider.mail)){
                    return NotFound("Mail address is invalid");
                }
                var token = await _demandService.update(offer);
                _mailService.sendConfirmationMail(token, offer.provider.mail, offer.provider.name);
                return Ok(token);
            } catch (UnknownAdressException e)
            {
                return NotFound(e.Message);
            }
        }


        //***********DELETE REQUESTS
        [HttpDelete("offers/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Delete(string token)
        {
            try
            {
                return Ok(await _demandService.delete(token));
            } catch(ArgumentException e)
            {
                return NotFound(e.Message);
            }
        }

    }
}
