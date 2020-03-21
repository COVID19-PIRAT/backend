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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] Consumable consumable)
        {
            return Ok(_service.queryProviders(consumable));
        }



        [HttpGet("devices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] Device device)
        {
            return Ok(_service.queryProviders(device));
        }

        [HttpGet("manpower")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] Manpower manpower)
        {
            return Ok(_service.queryProviders(manpower));
        }

        //*********POST REQUESTS


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Post([FromBody] Aggregate aggregate)
        {
            _service.update(aggregate);

            return Created("/successful", "Successful");
        }

    }
}
