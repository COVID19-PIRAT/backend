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

        private readonly IDemandService _service;

        public DemandController(ILogger<DemandController> logger, IDemandService service)
        {
            _logger = logger;
            _service = service;
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
                return Ok(await _service.QueryOffers(ConsumableEntity.of(consumable)));
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
                return Ok(await _service.QueryOffers(DeviceEntity.of(device)));
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
                return Ok(await _service.QueryOffers(manpower));
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
                return Ok(await _service.queryLink(token));
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
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody] Offer offer)
        {
            try
            {
                return Ok(await _service.update(offer));
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
                return Ok(await _service.delete(token));
            } catch(ArgumentException e)
            {
                return NotFound(e.Message);
            }
        }

    }
}
