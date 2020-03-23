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

        public DemandController(ILogger<DemandController> logger, IDemandService demandService, IMailService mailService)
        {
            _logger = logger;
            _demandService = demandService;
            _mailService = mailService;
        }

        //***********GET REQUESTS


        [HttpGet("consumables")]
        [ProducesResponseType(typeof(List<OfferItem<Consumable>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromQuery] Consumable consumable)
        {
            try {
                return Ok(await _demandService.QueryOffers(ConsumableEntity.of(consumable)));
            } catch (ArgumentException)
            {
                return BadRequest("Some obligatory value is missing");
            }
        }



        [HttpGet("devices")]
        [ProducesResponseType(typeof(List<OfferItem<Device>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromQuery] Device device)
        {
            try
            {
                return Ok(await _demandService.QueryOffers(DeviceEntity.of(device)));
            } catch (ArgumentException)
            {
                return BadRequest("Some obligatory value is missing");
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
            } catch (ArgumentException)
            {
                return BadRequest("Some obligatory value is missing");
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
        public async Task<IActionResult> Post([FromBody] Offer offer)
        {
            try
            {
                var token = await _demandService.update(offer);
                var host = Environment.GetEnvironmentVariable("PIRAT_HOST");
                if (string.IsNullOrEmpty(host))
                {
                    _logger.LogError("Could not find host");
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                var fullLink = $"http://{host}/resources/offers/{token}";
                await _mailService.sendConfirmationMail(fullLink, offer.provider.mail, offer.provider.name);
                return Ok(fullLink);
            } catch (MailException e)
            {
                return NotFound(e.Message);
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
