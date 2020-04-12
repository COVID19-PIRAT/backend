using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pirat.Exceptions;
using Pirat.Model.Api.Resource;
using Pirat.Other;
using Pirat.Services.Resource;
using Pirat.Services.Resource.Demands;

namespace Pirat.Controllers
{

    [ApiController]
    [Route("/demands")]
    public class DemandController : ControllerBase
    {

        private readonly ILogger<DemandController> _logger;

        private readonly IResourceDemandQueryService _resourceDemandQueryService;

        private readonly IResourceDemandInputValidatorService _resourceDemandInputValidatorService;


        public DemandController(
            ILogger<DemandController> logger,
            IResourceDemandQueryService resourceDemandQueryService,
            IResourceDemandInputValidatorService resourceDemandInputValidatorService
        )
        {
            _logger = logger;
            _resourceDemandQueryService = resourceDemandQueryService;
            _resourceDemandInputValidatorService = resourceDemandInputValidatorService;
        }

        /// <summary>
        /// Searches the demands and finds a list of consumables for the specified parameters of the query. 
        /// </summary>
        /// <param name="consumable"></param>
        /// <param name="address"></param>
        /// <returns>List of consumables</returns>
        /// <response code="200">Returns a list of consumables. Empty if no consumable found.</response>
        /// <response code="400">If arguments in the query are invalid.</response> 
        [HttpGet("consumable/search")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync([FromQuery] Consumable consumable, [FromQuery] Address address)
        {
            NullCheck.ThrowIfNull<Consumable>(consumable);
            NullCheck.ThrowIfNull<Address>(address);

            try
            {
                consumable.address = address;
                _resourceDemandInputValidatorService.ValidateForDemandQuery(consumable);
                return Ok(await _resourceDemandQueryService.QueryDemandsAsync(consumable).ToListAsync());
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (UnknownAdressException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Searches demands and finds a list of devices for the specified parameters of the query. 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="address"></param>
        /// <returns>List of devices</returns>
        /// <response code="200">Returns a list of devices. Empty if no device found.</response>
        /// <response code="400">If arguments in the query are invalid.</response> 
        [HttpGet("device/search")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAsync([FromQuery] Device device, [FromQuery] Address address)
        {
            NullCheck.ThrowIfNull<Device>(device);
            NullCheck.ThrowIfNull<Address>(address);

            try
            {
                device.address = address;
                _resourceDemandInputValidatorService.ValidateForDemandQuery(device);
                return Ok(await _resourceDemandQueryService.QueryDemandsAsync(device).ToListAsync());
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (UnknownAdressException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
