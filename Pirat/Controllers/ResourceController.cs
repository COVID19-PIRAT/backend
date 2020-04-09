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
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Pirat.Codes;
using Pirat.Extensions.Swagger.SwaggerConfiguration;
using Pirat.Model.Entity;
using Pirat.Services.Mail;
using Pirat.Services.Middleware;
using Pirat.Services.Resource;
using Pirat.SwaggerConfiguration;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Controllers
{

    [ApiController]
    [Route("/resources")]
    public class ResourceController : ControllerBase
    {

        private readonly ILogger<ResourceController> _logger;

        private readonly IResourceDemandService _resourceDemandService;

        private readonly IResourceUpdateService _resourceUpdateService;

        private readonly IResourceInputValidatorService _resourceInputValidatorService;

        private readonly IMailService _mailService;

        private readonly IMailInputValidatorService _mailInputValidatorService;

        private readonly IReCaptchaService _reCaptchaService;

        public ResourceController(
            ILogger<ResourceController> logger,
            IResourceDemandService resourceDemandService,
            IResourceUpdateService resourceUpdateService,
            IResourceInputValidatorService resourceInputValidatorService,
            IMailService mailService,
            IMailInputValidatorService mailInputValidatorService,
            IReCaptchaService reCaptchaService
            )
        {
            _logger = logger;
            _resourceDemandService = resourceDemandService;
            _resourceUpdateService = resourceUpdateService;
            _resourceInputValidatorService = resourceInputValidatorService;
            _mailService = mailService;
            _mailInputValidatorService = mailInputValidatorService;
            _reCaptchaService = reCaptchaService;
        }
        
        //***********GET REQUESTS

        /// <summary>
        /// Searches list of Consumables and the associated Provider. Provider only included if public.
        /// </summary>
        /// <param name="consumable"></param>
        /// <param name="address"></param>
        /// <returns>List of consumables</returns>
        /// <response code="200">Returns the list of consumables. Empty if no consumable found.</response>
        /// <response code="400">If arguments in the query are invalid.</response> 
        [HttpGet("consumables")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(List<OfferResource<Consumable>>))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OfferConsumableResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> Get([FromQuery] Consumable consumable, [FromQuery] Address address)
        {
            try
            {
                consumable.address = address;
                _resourceInputValidatorService.validateForQuery(consumable);
                return Ok(await _resourceDemandService.QueryOffers(consumable));
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
        /// Searches list of Devices and the associated Provider. Provider only included if public
        /// </summary>
        /// <param name="device"></param>
        /// <param name="address"></param>
        /// <returns>List of devices</returns>
        /// <response code="200">Returns the list of devices. Empty if no device found.</response>
        /// <response code="400">If arguments in the query are invalid.</response> 
        [HttpGet("devices")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(List<OfferResource<Device>>))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OfferDeviceResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> Get([FromQuery] Device device, [FromQuery] Address address)
        {
            try
            {
                device.address = address;
                _resourceInputValidatorService.validateForQuery(device);
                return Ok(await _resourceDemandService.QueryOffers(device));
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
        /// Searches list of Personals and the associated Provider. Provider only included if public.
        /// </summary>
        /// <param name="manpower"></param>
        /// <param name="address"></param>
        /// <returns>List of personals</returns>
        /// <response code="200">Returns the list of personals. Empty if no personal found.</response>
        /// <response code="400">If arguments in the query are invalid.</response> 
        [HttpGet("manpower")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(List<OfferResource<Personal>>))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OfferPersonalResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> Get([FromQuery] Manpower manpower, [FromQuery] Address address)
        {
            try
            {
                manpower.address = address;
                _resourceInputValidatorService.validateForQuery(manpower);
                return Ok(await _resourceDemandService.QueryOffers(manpower));
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
        /// Searches the Offer for the given token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>The offer of the token</returns>
        /// <response code="200">Returns the offer.</response>
        /// <response code="400">If token is invalid</response>
        /// <response code="404">If no offer for the token exists</response> 
        [HttpGet("offers/{token}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Offer))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OfferResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> Get(string token)
        {
            try
            {
                _resourceInputValidatorService.validateToken(token);
                return Ok(await _resourceDemandService.queryLink(token));
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }


        //*********POST REQUESTS

        /// <summary>
        /// Creates an offer.
        /// </summary>
        /// <param name="offer"></param>
        /// <returns>A newly created offer</returns>
        /// <response code="200">Returns the newly created offer</response>
        /// <response code="400">If data in the offer is invalid or not sufficient</response>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerRequestExample(typeof(Offer), typeof(OfferRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(Offer))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OfferResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> Post([FromBody] Offer offer)
        {
            try
            {
                _mailInputValidatorService.validateMail(offer.provider.mail);
                _resourceInputValidatorService.validateForDatabaseInsertion(offer);
                var token = await _resourceUpdateService.insert(offer);
                _mailService.sendNewOfferConfirmationMail(token, offer.provider.mail, offer.provider.name);
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
        /// Sending a demand request for the consumable with the given id to the non-public provider.
        /// </summary>
        /// <param name="contactInformationDemand"></param>
        /// <param name="id">The id of the consumable</param>
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
        public async Task<IActionResult> ConsumableAnonymousContact([FromBody] ContactInformationDemand contactInformationDemand, int id)
        {
            try
            {
                _mailInputValidatorService.validateMail(contactInformationDemand.senderEmail);

                var consumable = (ConsumableEntity) await _resourceDemandService.Find(new ConsumableEntity(), id);
                if (consumable is null)
                {
                    return NotFound(Error.ErrorCodes.NOTFOUND_CONSUMABLE);
                }

                var offer = (OfferEntity) await _resourceDemandService.Find(new OfferEntity(), consumable.offer_id);
                if (offer is null)
                {
                    return NotFound(Error.ErrorCodes.NOTFOUND_OFFER);
                }

                var mailAddressReceiver = offer.mail;
                var mailUserNameReceiver = offer.name;
                _mailService.sendDemandMailToProvider(contactInformationDemand, mailAddressReceiver,
                    mailUserNameReceiver);
                _mailService.sendDemandConformationMailToDemander(contactInformationDemand);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Sending a demand request for the device with the given id to the non-public provider.
        /// </summary>
        /// <param name="contactInformationDemand"></param>
        /// <param name="id">The id of the device</param>
        /// <returns>Empty string</returns>
        /// <response code="200">Empty string - consumable with provider found and mail has been sent</response>
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
        public async Task<IActionResult> DeviceAnonymContact([FromBody] ContactInformationDemand contactInformationDemand, int id)
        {
            try
            {
                _mailInputValidatorService.validateMail(contactInformationDemand.senderEmail);
                var device = (DeviceEntity) await _resourceDemandService.Find(new DeviceEntity(), id);
                if (device is null)
                {
                    return NotFound(Error.ErrorCodes.NOTFOUND_DEVICE);
                }

                var offer = (OfferEntity) await _resourceDemandService.Find(new OfferEntity(), device.offer_id);
                if (offer is null)
                {
                    return NotFound(Error.ErrorCodes.NOTFOUND_OFFER);
                }

                var mailAddressReceiver = offer.mail;
                var mailUserNameReceiver = offer.name;
                _mailService.sendDemandMailToProvider(contactInformationDemand, mailAddressReceiver,
                    mailUserNameReceiver);
                _mailService.sendDemandConformationMailToDemander(contactInformationDemand);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Sending a demand request for personal with the given id to the non-public provider.
        /// </summary>
        /// <param name="contactInformationDemand"></param>
        /// <param name="id">The id of the personal</param>
        /// <returns>Empty string</returns>
        /// <response code="200">Empty string - personal with provider found and mail has been sent</response>
        /// <response code="404">Resource does not exist</response>
        /// <response code="400">Invalid contact data</response>
        [HttpPost("manpower/{id:int}/contact")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerRequestExample(typeof(ContactInformationDemand), typeof(ContactInformationDemandExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> PersonalAnonymContact([FromBody] ContactInformationDemand contactInformationDemand, int id)
        {
            try
            {
                _mailInputValidatorService.validateMail(contactInformationDemand.senderEmail);
                var personal = (PersonalEntity) await _resourceDemandService.Find(new PersonalEntity(), id);
                if (personal is null)
                {
                    return NotFound(Error.ErrorCodes.NOTFOUND_PERSONAL);
                }

                var offer = (OfferEntity) await _resourceDemandService.Find(new OfferEntity(), personal.offer_id);
                if (offer is null)
                {
                    return NotFound(Error.ErrorCodes.NOTFOUND_OFFER);
                }

                var mailAddressReceiver = offer.mail;
                var mailUserNameReceiver = offer.name;
                _mailService.sendDemandMailToProvider(contactInformationDemand, mailAddressReceiver,
                    mailUserNameReceiver);
                _mailService.sendDemandConformationMailToDemander(contactInformationDemand);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }


        //***************PUT REQUESTS


        [HttpPut("offers/{token}/provider")]
        [Consumes("application/json")]
        [Produces("application/json")]
        // There seems to be a bug in the used swagger library. The following line fixes the shown example.
        [SwaggerRequestExample(typeof(string), typeof(ProviderRequestExample))]
        [SwaggerRequestExample(typeof(Provider), typeof(ProviderRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(void))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> ChangeProvider(string token, [FromBody] Provider provider)
        {
            try
            {
                _resourceInputValidatorService.validateForChangeInformation(token, provider);
                await _resourceUpdateService.ChangeInformation(token, provider);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidDataStateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }


        [HttpPut("offers/{token}/consumable/{id:int}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        // There seems to be a bug in the used swagger library. The following line fixes the shown example.
        [SwaggerRequestExample(typeof(string), typeof(ConsumableRequestExample))]
        [SwaggerRequestExample(typeof(Consumable), typeof(ConsumableRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> ChangeResource(string token, int id, [FromBody] Consumable consumable)
        {
            try
            {
                consumable.id = id;
                _resourceInputValidatorService.validateForChangeInformation(token, consumable);
                int changedRows = await _resourceUpdateService.ChangeInformation(token, consumable);
                return Ok(changedRows);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidDataStateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        [HttpPut("offers/{token}/device/{id:int}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        // There seems to be a bug in the used swagger library. The following line fixes the shown example.
        [SwaggerRequestExample(typeof(string), typeof(DeviceRequestExample))]
        [SwaggerRequestExample(typeof(Device), typeof(DeviceRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> ChangeResource(string token, int id, [FromBody] Device device)
        {
            try
            {
                device.id = id;
                _resourceInputValidatorService.validateForChangeInformation(token, device);
                int changedRows = await _resourceUpdateService.ChangeInformation(token, device);
                return Ok(changedRows);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidDataStateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        [HttpPut("offers/{token}/personal/{id:int}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        // There seems to be a bug in the used swagger library. The following line fixes the shown example.
        [SwaggerRequestExample(typeof(string), typeof(PersonalRequestExample))]
        [SwaggerRequestExample(typeof(Personal), typeof(PersonalRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> ChangeResource(string token, int id, [FromBody] Personal personal)
        {
            try
            {
                personal.id = id;
                _resourceInputValidatorService.validateForChangeInformation(token, personal);
                int changedRows = await _resourceUpdateService.ChangeInformation(token, personal);
                return Ok(changedRows);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidDataStateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        [HttpPut("offers/{token}/consumable/{id:int}/amount")]
        [Consumes("application/json")]
        [Produces("application/json")]
        // There seems to be a bug in the used swagger library. The following line fixes the shown example.
        [SwaggerRequestExample(typeof(string), typeof(AmountChangeRequestExample))]
        [SwaggerRequestExample(typeof(AmountChange), typeof(AmountChangeRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> ChangeAmountConsumable(string token, int id, [FromBody] AmountChange amountChange)
        {
            try
            {
                await _resourceUpdateService.ChangeConsumableAmount(token, id, amountChange.amount, amountChange.reason);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidDataStateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        [HttpPut("offers/{token}/device/{id:int}/amount")]
        [Consumes("application/json")]
        [Produces("application/json")]
        // There seems to be a bug in the used swagger library. The following line fixes the shown example.
        [SwaggerRequestExample(typeof(string), typeof(AmountChangeRequestExample))]
        [SwaggerRequestExample(typeof(AmountChange), typeof(AmountChangeRequestExample))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EmptyResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ErrorCodeResponseExample))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(ErrorCodeResponseExample))]
        public async Task<IActionResult> ChangeAmountDevice(string token, int id, [FromBody] AmountChange amountChange)
        {
            try
            {
                await _resourceUpdateService.ChangeDeviceAmount(token, id, amountChange.amount, amountChange.reason);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidDataStateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        [HttpDelete("offers/{token}/consumable/{id:int}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> MarkConsumableAsDeleted(string token, int id, [FromBody] string reason)
        {
            try
            {
                await _resourceUpdateService.MarkConsumableAsDeleted(token, id, reason);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidDataStateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        [HttpDelete("offers/{token}/device/{id:int}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> MarkDeviceAsDeleted(string token, int id, [FromBody] string reason)
        {
            try
            {
                await _resourceUpdateService.MarkDeviceAsDeleted(token, id, reason);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidDataStateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        [HttpDelete("offers/{token}/personal/{id:int}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> MarkPersonalAsDeleted(string token, int id, [FromBody] string reason)
        {
            try
            {
                await _resourceUpdateService.MarkPersonalAsDeleted(token, id, reason);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

    }
}
