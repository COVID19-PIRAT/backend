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
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Pirat.Codes;
using Pirat.Model.Entity;
using Pirat.SwaggerConfiguration;
using Swashbuckle.AspNetCore.Filters;

namespace Pirat.Controllers
{

    [ApiController]
    [Route("/resources")]
    public class DemandController : ControllerBase
    {

        private readonly ILogger<DemandController> _logger;

        private readonly IDemandService _demandService;

        private readonly IMailService _mailService;

        private readonly IReCaptchaService _reCaptchaService;

        public DemandController(
            ILogger<DemandController> logger,
            IDemandService demandService,
            IMailService mailService,
            IReCaptchaService reCaptchaService
            )
        {
            _logger = logger;
            _demandService = demandService;
            _mailService = mailService;
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
        [ProducesResponseType(typeof(List<OfferResource<Consumable>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromQuery] Consumable consumable, [FromQuery] Address address)
        {
            try
            {
                consumable.address = address;
                return Ok(await _demandService.QueryOffers(consumable));
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
        [ProducesResponseType(typeof(List<OfferResource<Device>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromQuery] Device device, [FromQuery] Address address)
        {
            try
            {
                device.address = address;
                return Ok(await _demandService.QueryOffers(device));
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
        /// Searches lift of Personals and the associated Provider. Provider only included if public.
        /// </summary>
        /// <param name="manpower"></param>
        /// <param name="address"></param>
        /// <returns>List of personals</returns>
        /// <response code="200">Returns the list of personals. Empty if no personal found.</response>
        /// <response code="400">If arguments in the query are invalid.</response> 
        [HttpGet("manpower")]
        [ProducesResponseType(typeof(List<OfferResource<Personal>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromQuery] Manpower manpower, [FromQuery] Address address)
        {
            try
            {
                manpower.address = address;
                return Ok(await _demandService.QueryOffers(manpower));
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
        [ProducesResponseType(typeof(Offer), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Get(string token)
        {
            try
            {
                return Ok(await _demandService.queryLink(token));
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
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /resources
        ///     {
        ///         "provider": {
        ///             "address": {
        ///                 "street": "Hauptstrasse",
        ///                 "streetnumber": "1",
        ///                 "postalcode": "80333",
        ///                 "city": "München",
        ///                 "country": "Deutschland",
        ///                 "latitude": 0,
        ///                 "longitude": 0
        ///     }
        /// </remarks>
        /// <param name="offer"></param>
        /// <returns>A newly created offer</returns>
        /// <response code="200">Returns the newly created offer</response>
        /// <response code="400">If data in the offer is invalid or not sufficient</response>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerRequestExample(typeof(Offer), typeof(OfferModelExample))]
        public async Task<IActionResult> Post([FromBody] Offer offer)
        {
            try
            {
                if (!_mailService.verifyMail(offer.provider.mail))
                {
                    return BadRequest(Error.ErrorCodes.INVALID_MAIL);
                }
                var token = await _demandService.insert(offer);
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

        [HttpPost("consumables/{id:int}/contact")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> ConsumableAnonymContact([FromBody] ContactInformationDemand contactInformationDemand, int id)
        {
            if (!_mailService.verifyMail(contactInformationDemand.senderEmail))
            {
                return BadRequest(Error.ErrorCodes.INVALID_MAIL);
            }
            var consumable = (ConsumableEntity)await _demandService.Find(new ConsumableEntity(), id);
            if (consumable is null)
            {
                return NotFound(Error.ErrorCodes.NOTFOUND_CONSUMABLE);
            }
            var offer = (OfferEntity)await _demandService.Find(new OfferEntity(), consumable.offer_id);
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

        [HttpPost("devices/{id:int}/contact")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> DeviceAnonymContact([FromBody] ContactInformationDemand contactInformationDemand, int id)
        {
            if (!_mailService.verifyMail(contactInformationDemand.senderEmail))
            {
                return BadRequest(Error.ErrorCodes.INVALID_MAIL);
            }
            var device = (DeviceEntity)await _demandService.Find(new DeviceEntity(), id);
            if (device is null)
            {
                return NotFound(Error.ErrorCodes.NOTFOUND_DEVICE);
            }
            var offer = (OfferEntity)await _demandService.Find(new OfferEntity(), device.offer_id);
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

        [HttpPost("manpower/{id:int}/contact")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> PersonalAnonymContact([FromBody] ContactInformationDemand contactInformationDemand, int id)
        {
            if (!_mailService.verifyMail(contactInformationDemand.senderEmail))
            {
                return BadRequest(Error.ErrorCodes.INVALID_MAIL);
            }
            var personal = (PersonalEntity)await _demandService.Find(new PersonalEntity(), id);
            if (personal is null)
            {
                return NotFound(Error.ErrorCodes.NOTFOUND_PERSONAL);
            }
            var offer = (OfferEntity)await _demandService.Find(new OfferEntity(), personal.offer_id);
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


        //***********DELETE REQUESTS
        [HttpDelete("offers/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Delete(string token)
        {
            try
            {
                return Ok(await _demandService.delete(token));
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
