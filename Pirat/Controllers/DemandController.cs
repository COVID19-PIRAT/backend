using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pirat.Codes;
using Pirat.Configuration;
using Pirat.Exceptions;
using Pirat.Extensions.Swagger.SwaggerConfiguration;
using Pirat.Model;
using Pirat.Model.Api.Admin;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Demands;
using Pirat.Model.Entity.Resource.Stock;
using Pirat.Other;
using Pirat.Services;
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
        
        private readonly IMailService _mailService;
        
        private readonly IConfigurationService _configurationService;

        private Language _languageDE;

        private Language _languageEN;

        private readonly string _adminKey;


        public DemandController(
            ILogger<DemandController> logger,
            IResourceDemandQueryService resourceDemandQueryService,
            IResourceDemandInputValidatorService resourceDemandInputValidatorService,
            IResourceDemandUpdateService resourceDemandUpdateService,
            IMailInputValidatorService mailInputValidatorService,
            IMailService mailService,
            IConfigurationService configurationService
        )
        {
            _logger = logger;
            _resourceDemandQueryService = resourceDemandQueryService;
            _resourceDemandInputValidatorService = resourceDemandInputValidatorService;
            _resourceDemandUpdateService = resourceDemandUpdateService;
            _mailInputValidatorService = mailInputValidatorService;
            _mailService = mailService;
            _configurationService = configurationService;

            _adminKey = Environment.GetEnvironmentVariable("PIRAT_ADMIN_KEY");
            _languageDE = configurationService.GetConfigForRegion("de").Languages["de"];
            _languageEN = configurationService.GetConfigForRegion("de").Languages["en"];
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
        
        
        /// <summary>
        /// Sending a offer request for the device with the given id to the non-public demander.
        /// </summary>
        /// <param name="contactInformationDemand"></param>
        /// <param name="id">The id of the device</param>
        /// <returns>Empty string</returns>
        /// <response code="200">Empty string - device with provider found and mail has been sent</response>
        /// <response code="404">Resource does not exist</response>
        /// <response code="400">Invalid contact data</response>
        [HttpPost("devices/{id:int}/contact")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerRequestExample(typeof(ContactInformationDemand), typeof(ContactInformationDemandExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> DeviceAnonymousContactAsync([FromBody] ContactInformationDemand contactInformationDemand, int id)
        {
            NullCheck.ThrowIfNull<ContactInformationDemand>(contactInformationDemand);

            try
            {
                _mailInputValidatorService.validateMail(contactInformationDemand.senderEmail);
                DeviceDemandEntity device = await _resourceDemandQueryService.FindAsync(new DeviceDemandEntity(), id);
                if (device is null)
                {
                    return NotFound(FailureCodes.NotFoundDevice);
                }

                DemandEntity demand = await _resourceDemandQueryService.FindAsync(new DemandEntity(), device.demand_id);
                if (demand is null)
                {
                    return NotFound(FailureCodes.NotFoundOffer);
                }

                var resourceNameDE = _languageDE.Device[device.category];
                var resourceNameEN = _languageEN.Device[device.category];

                var mailAddressReceiver = demand.mail;
                var mailUserNameReceiver = demand.name;
                // TODO
                var region = "de";
                await _mailService.SendOfferMailToDemanderAsync(region, contactInformationDemand, mailAddressReceiver,
                    mailUserNameReceiver, resourceNameDE, resourceNameEN);
                await _mailService.SendDemandConformationMailToDemanderAsync(region, contactInformationDemand);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
        
        
                /// <summary>
        /// Sending a offer request for the device with the given id to the non-public demander.
        /// </summary>
        /// <param name="contactInformationDemand"></param>
        /// <param name="id">The id of the device</param>
        /// <returns>Empty string</returns>
        /// <response code="200">Empty string - consumable with provider found and mail has been sent</response>
        /// <response code="404">Resource does not exist</response>
        /// <response code="400">Invalid contact data</response>
        [HttpPost("consumables/{id:int}/contact")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerRequestExample(typeof(ContactInformationDemand), typeof(ContactInformationDemandExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> ConsumableAnonymousContactAsync([FromBody] ContactInformationDemand contactInformationDemand, int id)
        {
            NullCheck.ThrowIfNull<ContactInformationDemand>(contactInformationDemand);

            try
            {
                _mailInputValidatorService.validateMail(contactInformationDemand.senderEmail);
                ConsumableDemandEntity consumable = await _resourceDemandQueryService.FindAsync(new ConsumableDemandEntity(), id);
                if (consumable is null)
                {
                    return NotFound(FailureCodes.NotFoundDevice);
                }

                DemandEntity demand = await _resourceDemandQueryService.FindAsync(new DemandEntity(), consumable.demand_id);
                if (demand is null)
                {
                    return NotFound(FailureCodes.NotFoundOffer);
                }

                var resourceNameDE = _languageDE.Consumable[consumable.category];
                var resourceNameEN = _languageEN.Consumable[consumable.category];

                var mailAddressReceiver = demand.mail;
                var mailUserNameReceiver = demand.name;
                // TODO
                var region = "de";
                await _mailService.SendOfferMailToDemanderAsync(region, contactInformationDemand, mailAddressReceiver,
                    mailUserNameReceiver, resourceNameDE, resourceNameEN);
                await _mailService.SendDemandConformationMailToDemanderAsync(region, contactInformationDemand);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
