using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pirat.Exceptions;
using Pirat.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Codes;
using Pirat.Extensions.Swagger.SwaggerConfiguration;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Stock;
using Pirat.Other;
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

        private readonly IResourceStockQueryService _resourceStockQueryService;

        private readonly IResourceStockUpdateService _resourceStockUpdateService;

        private readonly IResourceStockInputValidatorService _resourceStockInputValidatorService;

        private readonly IMailService _mailService;

        private readonly IMailInputValidatorService _mailInputValidatorService;

        private readonly IReCaptchaService _reCaptchaService;

        public ResourceController(
            ILogger<ResourceController> logger,
            IResourceStockQueryService resourceStockQueryService,
            IResourceStockUpdateService resourceStockUpdateService,
            IResourceStockInputValidatorService resourceStockInputValidatorService,
            IMailService mailService,
            IMailInputValidatorService mailInputValidatorService,
            IReCaptchaService reCaptchaService
            )
        {
            _logger = logger;
            _resourceStockQueryService = resourceStockQueryService;
            _resourceStockUpdateService = resourceStockUpdateService;
            _resourceStockInputValidatorService = resourceStockInputValidatorService;
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
        public async Task<IActionResult> GetAsync([FromQuery] Consumable consumable, [FromQuery] Address address)
        {
            NullCheck.ThrowIfNull<Consumable>(consumable);
            NullCheck.ThrowIfNull<Address>(address);

            try
            {
                consumable.address = address;
                _resourceStockInputValidatorService.ValidateForStockQuery(consumable);
                return Ok(await _resourceStockQueryService.QueryOffersAsync(consumable).ToListAsync());
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
        public async Task<IActionResult> GetAsync([FromQuery] Device device, [FromQuery] Address address)
        {
            NullCheck.ThrowIfNull<Device>(device);
            NullCheck.ThrowIfNull<Address>(address);

            try
            {
                device.address = address;
                _resourceStockInputValidatorService.ValidateForStockQuery(device);
                return Ok(await _resourceStockQueryService.QueryOffersAsync(device).ToListAsync());
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
        public async Task<IActionResult> GetAsync([FromQuery] Manpower manpower, [FromQuery] Address address)
        {
            NullCheck.ThrowIfNull<Manpower>(manpower);
            NullCheck.ThrowIfNull<Address>(address);

            try
            {
                manpower.address = address;
                _resourceStockInputValidatorService.ValidateForStockQuery(manpower);
                return Ok(await _resourceStockQueryService.QueryOffersAsync(manpower).ToListAsync());
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
        public async Task<IActionResult> GetAsync(string token)
        {
            try
            {
                _resourceStockInputValidatorService.ValidateToken(token);
                return Ok(await _resourceStockQueryService.QueryLinkAsync(token));
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
        public async Task<IActionResult> PostAsync([FromBody] Offer offer)
        {
            NullCheck.ThrowIfNull<Offer>(offer);

            try
            {
                _mailInputValidatorService.validateMail(offer.provider.mail);
                _resourceStockInputValidatorService.ValidateForStockInsertion(offer);
                var token = await _resourceStockUpdateService.InsertAsync(offer);
                await _mailService.SendNewOfferConfirmationMailAsync(token, offer.provider.mail, offer.provider.name);
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
        public async Task<IActionResult> ConsumableAnonymousContactAsync([FromBody] ContactInformationDemand contactInformationDemand, int id)
        {
            NullCheck.ThrowIfNull<ContactInformationDemand>(contactInformationDemand);

            try
            {
                _mailInputValidatorService.validateMail(contactInformationDemand.senderEmail);

                var consumable = (ConsumableEntity) await _resourceStockQueryService.FindAsync(new ConsumableEntity(), id);
                if (consumable is null)
                {
                    return NotFound(FailureCodes.NotFoundConsumable);
                }

                var offer = (OfferEntity) await _resourceStockQueryService.FindAsync(new OfferEntity(), consumable.offer_id);
                if (offer is null)
                {
                    return NotFound(FailureCodes.NotFoundOffer);
                }

                var mailAddressReceiver = offer.mail;
                var mailUserNameReceiver = offer.name;
                await _mailService.SendDemandMailToProviderAsync(contactInformationDemand, mailAddressReceiver,
                    mailUserNameReceiver);
                await _mailService.SendDemandConformationMailToDemanderAsync(contactInformationDemand);
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
        public async Task<IActionResult> DeviceAnonymContactAsync([FromBody] ContactInformationDemand contactInformationDemand, int id)
        {
            NullCheck.ThrowIfNull<ContactInformationDemand>(contactInformationDemand);

            try
            {
                _mailInputValidatorService.validateMail(contactInformationDemand.senderEmail);
                var device = (DeviceEntity) await _resourceStockQueryService.FindAsync(new DeviceEntity(), id);
                if (device is null)
                {
                    return NotFound(FailureCodes.NotFoundDevice);
                }

                var offer = (OfferEntity) await _resourceStockQueryService.FindAsync(new OfferEntity(), device.offer_id);
                if (offer is null)
                {
                    return NotFound(FailureCodes.NotFoundOffer);
                }

                var mailAddressReceiver = offer.mail;
                var mailUserNameReceiver = offer.name;
                await _mailService.SendDemandMailToProviderAsync(contactInformationDemand, mailAddressReceiver,
                    mailUserNameReceiver);
                await _mailService.SendDemandConformationMailToDemanderAsync(contactInformationDemand);
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
        public async Task<IActionResult> PersonalAnonymContactAsync([FromBody] ContactInformationDemand contactInformationDemand, int id)
        {
            NullCheck.ThrowIfNull<ContactInformationDemand>(contactInformationDemand);

            try
            {
                _mailInputValidatorService.validateMail(contactInformationDemand.senderEmail);
                var personal = (PersonalEntity) await _resourceStockQueryService.FindAsync(new PersonalEntity(), id);
                if (personal is null)
                {
                    return NotFound(FailureCodes.NotFoundPersonal);
                }

                var offer = (OfferEntity) await _resourceStockQueryService.FindAsync(new OfferEntity(), personal.offer_id);
                if (offer is null)
                {
                    return NotFound(FailureCodes.NotFoundOffer);
                }

                var mailAddressReceiver = offer.mail;
                var mailUserNameReceiver = offer.name;
                await _mailService.SendDemandMailToProviderAsync(contactInformationDemand, mailAddressReceiver,
                    mailUserNameReceiver);
                await _mailService.SendDemandConformationMailToDemanderAsync(contactInformationDemand);
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
        public async Task<IActionResult> ChangeProviderAsync(string token, [FromBody] Provider provider)
        {
            try
            {
                _resourceStockInputValidatorService.ValidateForChangeInformation(token, provider);
                await _resourceStockUpdateService.ChangeInformationAsync(token, provider);
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
        public async Task<IActionResult> ChangeResourceAsync(string token, int id, [FromBody] Consumable consumable)
        {
            NullCheck.ThrowIfNull<Consumable>(consumable);

            try
            {
                consumable.id = id;
                _resourceStockInputValidatorService.ValidateForChangeInformation(token, consumable);
                int changedRows = await _resourceStockUpdateService.ChangeInformationAsync(token, consumable);
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
        public async Task<IActionResult> ChangeResourceAsync(string token, int id, [FromBody] Device device)
        {
            NullCheck.ThrowIfNull<Device>(device);

            try
            {
                device.id = id;
                _resourceStockInputValidatorService.ValidateForChangeInformation(token, device);
                int changedRows = await _resourceStockUpdateService.ChangeInformationAsync(token, device);
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
        public async Task<IActionResult> ChangeResourceAsync(string token, int id, [FromBody] Personal personal)
        {
            NullCheck.ThrowIfNull<Personal>(personal);

            try
            {
                personal.id = id;
                _resourceStockInputValidatorService.ValidateForChangeInformation(token, personal);
                int changedRows = await _resourceStockUpdateService.ChangeInformationAsync(token, personal);
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
        public async Task<IActionResult> ChangeAmountConsumableAsync(string token, int id, [FromBody] AmountChange amountChange)
        {
            NullCheck.ThrowIfNull<AmountChange>(amountChange);

            try
            {
                await _resourceStockUpdateService.ChangeConsumableAmountAsync(token, id, amountChange.amount, amountChange.reason);
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
        public async Task<IActionResult> ChangeAmountDeviceAsync(string token, int id, [FromBody] AmountChange amountChange)
        {
            NullCheck.ThrowIfNull<AmountChange>(amountChange);

            try
            {
                await _resourceStockUpdateService.ChangeDeviceAmountAsync(token, id, amountChange.amount, amountChange.reason);
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
        public async Task<IActionResult> MarkConsumableAsDeletedAsync(string token, int id, [FromBody] string reason)
        {
            try
            {
                await _resourceStockUpdateService.MarkConsumableAsDeletedAsync(token, id, reason);
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
        public async Task<IActionResult> MarkDeviceAsDeletedAsync(string token, int id, [FromBody] string reason)
        {
            try
            {
                await _resourceStockUpdateService.MarkDeviceAsDeletedAsync(token, id, reason);
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
        public async Task<IActionResult> MarkPersonalAsDeletedAsync(string token, int id, [FromBody] string reason)
        {
            try
            {
                await _resourceStockUpdateService.MarkPersonalAsDeletedAsync(token, id, reason);
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

        [HttpPost("offers/{token}/consumable")]
        [Consumes("application/json")]
        [Produces("application/json")]
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
        public async Task<IActionResult> AddResourceAsync(string token, [FromBody] Consumable consumable)
        {
            try
            {
                _resourceStockInputValidatorService.ValidateForStockInsertion(consumable);
                await _resourceStockUpdateService.AddResourceAsync(token, consumable);
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

        [HttpPost("offers/{token}/device")]
        [Consumes("application/json")]
        [Produces("application/json")]
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
        public async Task<IActionResult> AddResourceAsync(string token, [FromBody] Device device)
        {
            try
            {
                _resourceStockInputValidatorService.ValidateForStockInsertion(device);
                await _resourceStockUpdateService.AddResourceAsync(token, device);
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

        [HttpPost("offers/{token}/manpower")]
        [Consumes("application/json")]
        [Produces("application/json")]
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
        public async Task<IActionResult> AddResourceAsync(string token, [FromBody] Personal personal)
        {
            try
            {
                _resourceStockInputValidatorService.ValidateForStockInsertion(personal);
                await _resourceStockUpdateService.AddResourceAsync(token, personal);
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


    }
}
