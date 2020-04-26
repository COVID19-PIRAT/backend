using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pirat.Codes;
using Pirat.DatabaseContext;
using Pirat.Exceptions;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Stock;
using Pirat.Other;
using Pirat.Services.Helper.AddressMaking;

namespace Pirat.Services.Resource
{
    public class ResourceStockUpdateService : IResourceStockUpdateService
    {
        private readonly ILogger<ResourceStockUpdateService> _logger;

        private readonly ResourceContext _context;

        private readonly IAddressMaker _addressMaker;

        private readonly QueryHelper _queryHelper;


        public ResourceStockUpdateService(ILogger<ResourceStockUpdateService> logger, ResourceContext context, IAddressMaker addressMaker)
        {
            _logger = logger;
            _context = context;
            _addressMaker = addressMaker;

            _queryHelper = new QueryHelper(context);

        }

        /// <summary>
        /// Inserts a consumable entity into the database which gets <c>offerId</c> as foreign key for the offer entity it relates to.
        /// </summary>
        /// <param name="offerId">The unique id of the offer</param>
        /// <param name="consumable">The consumable from which the entity is created</param>
        /// <returns></returns>
        private async Task InsertAsync(int offerId, Consumable consumable)
        {
            var consumableEntity = new ConsumableEntity().Build(consumable);
            consumableEntity.offer_id = offerId;
            await consumableEntity.InsertAsync(_context);
            consumable.id = consumableEntity.id;
        }

        /// <summary>
        /// Inserts a device entity into the database which gets <c>offerId</c> as foreign key for the offer entity it relates to
        /// </summary>
        /// <param name="offerId">The unique id of the offer</param>
        /// <param name="device">The device from which the entity is created</param>
        /// <returns></returns>
        private async Task InsertAsync(int offerId, Device device)
        {
            var deviceEntity = new DeviceEntity().Build(device);
            deviceEntity.offer_id = offerId;
            await deviceEntity.InsertAsync(_context);
            device.id = deviceEntity.id;
        }

        /// <summary>
        /// Inserts a personal entity into the database which gets <c>offerId</c> as foreign key for the personal entity it relates to.
        /// </summary>
        /// <param name="offerId">The unique id of the offer</param>
        /// <param name="personal">The personal from which the entity is created</param>
        /// <returns></returns>
        private async Task InsertAsync(int offerId, Personal personal)
        {
            var personEntity = new PersonalEntity().Build(personal);
            personEntity.offer_id = offerId;
            await personEntity.InsertAsync(_context);
            personal.id = personEntity.id;
        }


        public async Task<string> InsertAsync(Offer offer, string region)
        {
            NullCheck.ThrowIfNull<Offer>(offer);

            var provider = offer.provider;

            //Build as entities

            var offerEntity = new OfferEntity().Build(provider, region);
            var offerAddressEntity = new AddressEntity().build(provider.address);

            //Create the coordinates and store the address of the offer

            _addressMaker.SetCoordinates(offerAddressEntity);
            await offerAddressEntity.InsertAsync(_context);

            //Store the offer including the address id as foreign key, the token and a timestamp
            offerEntity.address_id = offerAddressEntity.Id;
            offerEntity.token = createToken();
            offerEntity.timestamp = DateTime.Now;
            await offerEntity.InsertAsync(_context);

            //create the entities for the resources, calculate their coordinates, give them the offer foreign key and store them
            //Update the original offer with the ids from the created entities (helps us for testing and if we want to do more stuff with the offer in future features)

            int offer_id = offerEntity.id;

            if (!(offer.consumables is null))
            {
                foreach (var c in offer.consumables)
                {
                    await InsertAsync(offer_id, c);
                }
            }
            if (!(offer.personals is null))
            {
                foreach (var p in offer.personals)
                {
                    await InsertAsync(offer_id, p);
                }
            }
            if (!(offer.devices is null))
            {
                foreach (var d in offer.devices)
                {
                    await InsertAsync(offer_id, d);
                }
            }

            //Give back only the token
            return offerEntity.token;
        }

        public async Task DeleteAsync(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Length != Constants.OfferTokenLength)
            {
                throw new ArgumentException(FailureCodes.InvalidToken);
            }

            var offer = await _queryHelper.RetrieveOfferFromTokenAsync(token);

            //Delete the offer. The resources have the offer id as foreign key and get deleted as well by the db.
            await offer.DeleteAsync(_context);
        }

        public async Task<int> ChangeInformationAsync(string token, Provider provider)
        {
            NullCheck.ThrowIfNull<Provider>(provider);

            AddressEntity location = new AddressEntity().build(provider.address);
            _addressMaker.SetCoordinates(location);

            var query = 
                from o in _context.offer as IQueryable<OfferEntity>
                join ap in _context.address on o.address_id equals ap.Id
                where o.token == token
                select new {o, ap};

            foreach(var collection in await query.ToListAsync())
            {
                collection.o.name = provider.name;
                //o.ispublic = provider.ispublic; //TODO everything non public so far
                collection.o.organisation = provider.organisation;
                collection.o.phone = provider.phone;
                collection.ap.OverwriteWith(location);
            }
            
            var changedRows = await _context.SaveChangesAsync();

            if (2 < changedRows)
            {
                throw new InvalidDataStateException(FatalCodes.UpdatesMadeInTooManyRows);
            }

            return changedRows;
        }

        public async Task<int> ChangeInformationAsync(string token, Consumable consumable)
        {
            NullCheck.ThrowIfNull<Consumable>(consumable);
            
            var query = 
                from o in _context.offer as IQueryable<OfferEntity>
                join c in _context.consumable on o.id equals c.offer_id
                where o.token == token && c.id == consumable.id
                select new {o, c};

            foreach(var collection in await query.ToListAsync())
            {
                collection.c.annotation = consumable.annotation;
                collection.c.unit = consumable.unit;
                collection.c.name = consumable.name;
                collection.c.manufacturer = consumable.manufacturer;
                collection.c.ordernumber = consumable.ordernumber;
            }

            var changedRows = await _context.SaveChangesAsync();

            if (1 < changedRows)
            {
                throw new InvalidDataStateException(FatalCodes.UpdatesMadeInTooManyRows);
            }

            return changedRows;
        }

        public async Task<int> ChangeInformationAsync(string token, Device device)
        {
            NullCheck.ThrowIfNull<Device>(device);
            
            var query = 
                from o in _context.offer as IQueryable<OfferEntity>
                join d in _context.device on o.id equals d.offer_id
                where o.token == token && d.id == device.id
                select new { o, d };

            foreach (var collection in await query.ToListAsync())
            {
                collection.d.annotation = device.annotation;
                collection.d.name = device.name;
                collection.d.manufacturer = device.manufacturer;
                collection.d.ordernumber = device.ordernumber;
            }

            var changedRows = await _context.SaveChangesAsync();

            if (1 < changedRows)
            {
                throw new InvalidDataStateException(FatalCodes.UpdatesMadeInTooManyRows);
            }

            return changedRows;
        }

        public async Task<int> ChangeInformationAsync(string token, Personal personal)
        {
            NullCheck.ThrowIfNull<Personal>(personal);
            
            var query = 
                from o in _context.offer as IQueryable<OfferEntity>
                join p in _context.personal on o.id equals p.offer_id
                where o.token == token && p.id == personal.id
                select new {o, p};

            foreach (var collection in await query.ToListAsync())
            {
                collection.p.qualification = personal.qualification;
                collection.p.institution = personal.institution;
                collection.p.area = personal.area;
                collection.p.researchgroup = personal.researchgroup;
                collection.p.annotation = personal.annotation;
                collection.p.experience_rt_pcr = personal.experience_rt_pcr;
            }

            var changedRows = await _context.SaveChangesAsync();

            if (1 < changedRows)
            {
                throw new InvalidDataStateException(FatalCodes.UpdatesMadeInTooManyRows);
            }

            return changedRows;
        }

        public Task ChangeConsumableAmountAsync(string token, int consumableId, int newAmount)
        {
            return ChangeConsumableAmountAsync(token, consumableId, newAmount, "");
        }

        public async Task ChangeConsumableAmountAsync(string token, int consumableId, int newAmount, string reason)
        {
            NullCheck.ThrowIfNull<string>(token);
            NullCheck.ThrowIfNull<string>(reason);

            // Get consumable from database
            var query = 
                from o in _context.offer as IQueryable<OfferEntity>
                join c in _context.consumable on o.id equals c.offer_id
                where token == o.token && c.id == consumableId
                select new { c, o };
            var foundConsumables = await query.ToListAsync();

            if (foundConsumables.Count == 0)
            {
                throw new DataNotFoundException(FailureCodes.NotFoundConsumable);
            }
            ConsumableEntity consumable = foundConsumables[0].c;
            OfferEntity offer = foundConsumables[0].o;
            
            // If amount has not changed: do nothing
            if (consumable.amount == newAmount)
            {
                return;
            }

            int diffAmount = Math.Abs(newAmount - consumable.amount);
            
            // If amount has increased: no reason required
            if (consumable.amount < newAmount)
            {
                consumable.amount = newAmount;
                await consumable.UpdateAsync(_context);
                
                // Add log
                await new ChangeEntity()
                {
                    change_type = "INCREASE_AMOUNT",
                    element_id = consumable.id,
                    element_type = "consumable",
                    element_category = consumable.category,
                    element_name = consumable.name,
                    diff_amount = diffAmount,
                    reason = reason,
                    timestamp = DateTime.Now,
                    region = offer.region
                }.InsertAsync(_context);
                
                return;
            }
            
            // If amount has decreased: ensure that a reason is provided
            if (reason.Trim().Length == 0)
            {
                throw new ArgumentException(FailureCodes.InvalidReason);
            }
            if (newAmount < 1)
            {
                throw new ArgumentException(FailureCodes.InvalidAmountConsumable);
            }
            consumable.amount = newAmount;
            await consumable.UpdateAsync(_context);
            
            // Add log
            await new ChangeEntity()
            {
                change_type = "DECREASE_AMOUNT",
                element_id = consumable.id,
                element_type = "consumable",
                element_category = consumable.category,
                element_name = consumable.name,
                diff_amount = diffAmount,
                reason = reason,
                timestamp = DateTime.Now,
                region = offer.region
            }.InsertAsync(_context);
        }

        public Task ChangeDeviceAmountAsync(string token, int deviceId, int newAmount)
        {
            return ChangeDeviceAmountAsync(token, deviceId, newAmount, null);
        }

        public async Task ChangeDeviceAmountAsync(string token, int deviceId, int newAmount, string reason)
        {
            NullCheck.ThrowIfNull<string>(token);

            // Get device from database
            var query = 
                from o in _context.offer as IQueryable<OfferEntity>
                join d in _context.device on o.id equals d.offer_id
                where token == o.token && d.id == deviceId
                select new { d, o };

            var foundDevices = await query.ToListAsync();

            if (foundDevices.Count == 0)
            {
                throw new DataNotFoundException(FailureCodes.NotFoundDevice);
            }

            DeviceEntity device = foundDevices[0].d;
            OfferEntity offer = foundDevices[0].o;
            
            // If amount has not changed: do nothing
            if (device.amount == newAmount)
            {
                return;
            }

            int diffAmount = Math.Abs(newAmount - device.amount);

            // If amount has increased: no reason required
            if (device.amount < newAmount)
            {
                device.amount = newAmount;
                await device.UpdateAsync(_context);
                
                // Add log
                await new ChangeEntity()
                {
                    change_type = "INCREASE_AMOUNT",
                    element_id = device.id,
                    element_type = "device",
                    element_category = device.category,
                    element_name = device.name,
                    diff_amount = diffAmount,
                    reason = reason,
                    timestamp = DateTime.Now,
                    region = offer.region
                }.InsertAsync(_context);
                
                return;
            }

            NullCheck.ThrowIfNull<string>(reason);

            // If amount has decreased: ensure that a reason is provided
            if (reason.Trim().Length == 0)
            {
                throw new ArgumentException(FailureCodes.InvalidReason);
            }
            if (newAmount < 1)
            {
                throw new ArgumentException(FailureCodes.InvalidAmountDevice);
            }
            device.amount = newAmount;
            await device.UpdateAsync(_context);

            // Add log
            await new ChangeEntity()
            {
                change_type = "DECREASE_AMOUNT",
                element_id = device.id,
                element_type = "device",
                element_category = device.category,
                element_name = device.name,
                diff_amount = diffAmount,
                reason = reason,
                timestamp = DateTime.Now,
                region = offer.region
            }.InsertAsync(_context);
        }

        public async Task AddResourceAsync(string token, Consumable consumable)
        {
            NullCheck.ThrowIfNull<Consumable>(consumable);

            OfferEntity offerEntity = await _queryHelper.RetrieveOfferFromTokenAsync(token);

            await InsertAsync(offerEntity.id, consumable);
        }

        public async Task AddResourceAsync(string token, Device device)
        {
            NullCheck.ThrowIfNull<Device>(device);

            OfferEntity offerEntity = await _queryHelper.RetrieveOfferFromTokenAsync(token);

            await InsertAsync(offerEntity.id, device);
        }

        public async Task AddResourceAsync(string token, Personal personal)
        {
            NullCheck.ThrowIfNull<Personal>(personal);

            OfferEntity offerEntity = await _queryHelper.RetrieveOfferFromTokenAsync(token);

            await InsertAsync(offerEntity.id, personal);
        }

        public async Task MarkConsumableAsDeletedAsync(string token, int consumableId, string reason)
        {
            NullCheck.ThrowIfNull<string>(reason);

            if (reason.Trim().Length == 0)
            {
                throw new ArgumentException(FailureCodes.InvalidReason);
            }
            
            // Get consumable from database
            var query = 
                from o in _context.offer as IQueryable<OfferEntity>
                join c in _context.consumable on o.id equals c.offer_id
                where token == o.token && c.id == consumableId
                select new { c, o };

            var foundConsumables = await query.ToListAsync();

            if (foundConsumables.Count == 0)
            {
                throw new DataNotFoundException(FailureCodes.NotFoundConsumable);
            }

            ConsumableEntity consumableEntity = foundConsumables[0].c;
            OfferEntity offerEntity = foundConsumables[0].o;
            
            consumableEntity.is_deleted = true;
            await consumableEntity.UpdateAsync(_context);
            
            await new ChangeEntity()
            {
                change_type = ChangeEntityChangeType.DeleteResource,
                element_id = consumableEntity.id,
                element_type = ChangeEntityElementType.Consumable,
                element_category = consumableEntity.category,
                element_name = consumableEntity.name,
                diff_amount = consumableEntity.amount,
                reason = reason,
                timestamp = DateTime.Now,
                region = offerEntity.region
            }.InsertAsync(_context);
        }

        public async Task MarkDeviceAsDeletedAsync(string token, int deviceId, string reason)
        {
            NullCheck.ThrowIfNull<string>(token);
            NullCheck.ThrowIfNull<string>(reason);

            if (reason.Trim().Length == 0)
            {
                throw new ArgumentException(FailureCodes.InvalidReason);
            }

            // Get device from database
            var query = 
                from o in _context.offer as IQueryable<OfferEntity>
                join d in _context.device on o.id equals d.offer_id
                where token == o.token && d.id == deviceId
                select new { d, o };

            var foundDevices = await query.ToListAsync();

            if (foundDevices.Count == 0)
            {
                throw new DataNotFoundException(FailureCodes.NotFoundConsumable);
            }

            DeviceEntity deviceEntity = foundDevices[0].d;
            OfferEntity offerEntity = foundDevices[0].o;

            deviceEntity.is_deleted = true;
            await deviceEntity.UpdateAsync(_context);
            
            await new ChangeEntity()
            {
                change_type = ChangeEntityChangeType.DeleteResource,
                element_id = deviceEntity.id,
                element_type = ChangeEntityElementType.Device,
                element_category = deviceEntity.category,
                element_name = deviceEntity.name,
                diff_amount = deviceEntity.amount,
                reason = reason,
                timestamp = DateTime.Now,
                region = offerEntity.region
            }.InsertAsync(_context);
        }

        public async Task MarkPersonalAsDeletedAsync(string token, int personalId, string reason)
        {
            NullCheck.ThrowIfNull<string>(reason);

            if (reason.Trim().Length == 0)
            {
                throw new ArgumentException(FailureCodes.InvalidReason);
            }

            // Get personal from database
            var query = 
                from o in _context.offer as IQueryable<OfferEntity>
                join p in _context.personal on o.id equals p.offer_id
                where token == o.token && p.id == personalId
                select new { p, o };

            var foundPersonals = await query.ToListAsync();

            if (foundPersonals.Count == 0)
            {
                throw new DataNotFoundException(FailureCodes.NotFoundPersonal);
            }

            PersonalEntity personalEntity = foundPersonals[0].p;
            OfferEntity offerEntity = foundPersonals[0].o;

            personalEntity.is_deleted = true;
            await personalEntity.UpdateAsync(_context);

            await new ChangeEntity()
            {
                change_type = ChangeEntityChangeType.DeleteResource,
                element_id = personalEntity.id,
                element_type = ChangeEntityElementType.Personal,
                element_category = personalEntity.qualification,
                element_name = null,
                diff_amount = 1,
                reason = reason,
                timestamp = DateTime.Now,
                region = offerEntity.region
            }.InsertAsync(_context);
        }

        private string createToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            StringBuilder sb = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                while (sb.Length != Constants.OfferTokenLength)
                {
                    byte[] oneByte = new byte[1];
                    rng.GetBytes(oneByte);
                    char randomCharacter = (char)oneByte[0];
                    if (chars.Contains(randomCharacter, StringComparison.Ordinal))
                    {
                        sb.Append(randomCharacter);
                    }
                }
            }
            return sb.ToString();
        }
    }
}
