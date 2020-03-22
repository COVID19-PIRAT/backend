using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pirat.DatabaseContext;
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
        [ProducesResponseType(typeof(HashSet<Provider>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] Consumable consumable)
        {
            try { 
                return Ok(_service.queryProviders(consumable));
            } catch (ArgumentException)
            {
                return BadRequest("Some obligatory value is missing");
            }
}



        [HttpGet("devices")]
        [ProducesResponseType(typeof(HashSet<Provider>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] Device device)
        {
            try
            {
                return Ok(_service.queryProviders(device));
            } catch (ArgumentException)
            {
                return BadRequest("Some obligatory value is missing");
            }
        }

        [HttpGet("manpower")]
        [ProducesResponseType(typeof(HashSet<Provider>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] Manpower manpower)
        {
            try
            {
                return Ok(_service.queryProviders(manpower));
            } catch (ArgumentException)
            {
                return BadRequest("Some obligatory value is missing");
            }
           
        }

        [HttpGet("offers/{link}")]
        [ProducesResponseType(typeof(Aggregate), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Get(string link)
        {

            return Ok(_service.queryLink(link));

        }

        //*********POST REQUESTS


        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Post([FromBody] Offer offer)
        {
            return Ok(_service.update(offer));
        }

    }
}
