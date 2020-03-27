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
using Pirat.Model.Entity;

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


        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromBody] Offer offer)
        {
            try
            {
                if (!_mailService.verifyMail(offer.provider.mail))
                {
                    return BadRequest("Mail address is invalid");
                }

                var token = await _demandService.insert(offer);
                _mailService.sendConfirmationMail(token, offer.provider.mail, offer.provider.name);
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
                return BadRequest("Mail address invalid");
            }
            var consumable = (ConsumableEntity)await _demandService.Find(new ConsumableEntity(), id);
            if (consumable is null)
            {
                return NotFound($"Consumable {id} not found");
            }
            var offer = (OfferEntity)await _demandService.Find(new OfferEntity(), consumable.offer_id);
            if (offer is null)
            {
                return NotFound($"Offer from consumable {id} not found");
            }
            var mailAddressReceiver = offer.mail;
            var mailUserNameReceiver = offer.name;
            _mailService.sendDemandMailToProvider(contactInformationDemand, mailAddressReceiver,
                mailUserNameReceiver);
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
                return BadRequest("Mail address is invalid");
            }
            var device = (DeviceEntity)await _demandService.Find(new DeviceEntity(), id);
            if (device is null)
            {
                return NotFound($"Device {id} not found");
            }
            var offer = (OfferEntity)await _demandService.Find(new OfferEntity(), device.offer_id);
            if (offer is null)
            {
                return NotFound($"Offer from device {id} not found");
            }
            var mailAddressReceiver = offer.mail;
            var mailUserNameReceiver = offer.name;
            _mailService.sendDemandMailToProvider(contactInformationDemand, mailAddressReceiver,
                mailUserNameReceiver);
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
                return BadRequest("Mail address is invalid");
            }
            var personal = (PersonalEntity)await _demandService.Find(new PersonalEntity(), id);
            if (personal is null)
            {
                return NotFound($"Personal {id} not found");
            }
            var offer = (OfferEntity)await _demandService.Find(new OfferEntity(), personal.offer_id);
            if (offer is null)
            {
                return NotFound($"Offer from personal {id} not found");
            }
            var mailAddressReceiver = offer.mail;
            var mailUserNameReceiver = offer.name;
            _mailService.sendDemandMailToProvider(contactInformationDemand, mailAddressReceiver,
                mailUserNameReceiver);
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
