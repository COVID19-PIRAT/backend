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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Get()
        {
            return Ok("Resources");
        }

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


        [HttpPost("consumables")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Post([FromBody] Consumable consumable)
        {
            _service.update(consumable);

            return Ok("d");
        }

        [HttpPost("devices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Post([FromBody] Device device)
        {
            _service.update(device);

            return Ok("d");
        }

        [HttpPost("manpower")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Post([FromBody] Manpower manpower)
        {
            _service.update(manpower);

            return Ok("d");
        }

        [HttpPost("providers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Post([FromBody] Provider provider)
        {
            _service.update(provider);

            return Ok("d");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Post([FromBody] Aggregate aggregate)
        {
            _service.update(aggregate);

            //List<Device> devices = JsonConvert.DeserializeObject<List<Device>>(data["provider"].ToString());
            //Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

            //Provider provider = data["provider"].ToObject<Provider>();
            //Device[] devices = (JArray)data["devices"];
            //Provider provider = data["provider"].ToObject<Provider>();
            //Provider provider = data["provider"].ToObject<Provider>();

            return Ok("d");
        }

    }
}
