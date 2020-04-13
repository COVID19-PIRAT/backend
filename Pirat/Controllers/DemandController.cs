using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pirat.Exceptions;
using Pirat.Extensions.Swagger.SwaggerConfiguration;
using Pirat.Model.Api.Admin;
using Pirat.Model.Api.Resource;
using Pirat.Other;
using Pirat.Services.Mail;
using Pirat.Services.Resource;
using Pirat.Services.Resource.Demands;
using Pirat.SwaggerConfiguration;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Controllers
{
    [ApiController]
    [Route("/demands")]
    public class DemandController : ControllerBase
    {
        private readonly ILogger<DemandController> _logger;

        private readonly IResourceDemandQueryService _resourceDemandQueryService;

        private readonly IResourceDemandInputValidatorService _resourceDemandInputValidatorService;
        
        private readonly IResourceDemandUpdateService _resourceDemandUpdateService;

        private readonly IMailInputValidatorService _mailInputValidatorService;
        private readonly string _adminKey;


        public DemandController(
            ILogger<DemandController> logger,
            IResourceDemandQueryService resourceDemandQueryService,
            IResourceDemandInputValidatorService resourceDemandInputValidatorService,
            IResourceDemandUpdateService resourceDemandUpdateService,
            IMailInputValidatorService mailInputValidatorService
        )
        {
            _logger = logger;
            _resourceDemandQueryService = resourceDemandQueryService;
            _resourceDemandInputValidatorService = resourceDemandInputValidatorService;
            _resourceDemandUpdateService = resourceDemandUpdateService;
            _mailInputValidatorService = mailInputValidatorService;
            
            _adminKey = Environment.GetEnvironmentVariable("PIRAT_ADMIN_KEY");
        }

        /// <summary>
        /// Creates a demand
        /// </summary>
        /// <returns code="200">The secret token</returns>
        /// <response code="400">If data in the offer is invalid or not sufficient</response>
        /// <response code="403">If the admin key is invalid</response>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerRequestExample(typeof(AdminKeyProtected<Demand>), typeof(DemandRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> PostAsync([FromBody] AdminKeyProtected<Demand> adminKeyProtectedDemand)
        {
            NullCheck.ThrowIfNull<Demand>(adminKeyProtectedDemand);

            // This is a temporary solution until #103 is done.
            if (!_adminKey.Equals(adminKeyProtectedDemand.adminKey, StringComparison.InvariantCulture))
            {
                return StatusCode(403);
            }
            
            Demand demand = adminKeyProtectedDemand.data;
            try
            {
                _mailInputValidatorService.validateMail(demand.provider.mail);
                _resourceDemandInputValidatorService.ValidateForDemandInsertion(demand);
                var token = await _resourceDemandUpdateService.InsertAsync(demand);
                return Ok(token);
            }
            catch (UnknownAdressException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
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
