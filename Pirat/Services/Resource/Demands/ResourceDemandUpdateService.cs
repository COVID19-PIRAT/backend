using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Pirat.DatabaseContext;
using Pirat.Model.Api.Resource;
using Pirat.Model.Entity.Resource.Common;
using Pirat.Model.Entity.Resource.Demands;
using Pirat.Model.Entity.Resource.Stock;
using Pirat.Other;
using Pirat.Services.Helper.AddressMaking;


namespace Pirat.Services.Resource.Demands
{
    public class ResourceDemandUpdateService : IResourceDemandUpdateService
    {
        private readonly ResourceContext _context;
        private readonly IAddressMaker _addressMaker;

        public ResourceDemandUpdateService(ResourceContext context, IAddressMaker addressMaker)
        {
            _context = context;
            _addressMaker = addressMaker;
        }

        public async Task<string> InsertAsync(Demand demand)
        {
            NullCheck.ThrowIfNull<Offer>(demand);
            Provider provider = demand.provider;

            //Build as entities
            DemandEntity demandEntity = new DemandEntity().Build(provider);

            // Store address if available
            if (provider.address != null && provider.address.ContainsInformation())
            {
                AddressEntity demandAddressEntity = new AddressEntity().build(provider.address);
                _addressMaker.SetCoordinates(demandAddressEntity);
                await demandAddressEntity.InsertAsync(_context);
                demandEntity.address_id = demandAddressEntity.Id;
            }
            
            // Store demand
            demandEntity.created_at_timestamp = DateTime.Now;
            demandEntity.token = CreateToken();
            await demandEntity.InsertAsync(_context);
            var demandId = demandEntity.id;

            // Store resources
            if (demand.consumables != null)
            {
                foreach (Consumable c in demand.consumables)
                {
                    ConsumableDemandEntity entity = new ConsumableDemandEntity().Build(c);
                    entity.demand_id = demandId;
                    entity.created_at_timestamp = DateTime.Now;
                    await entity.InsertAsync(_context);
                }
            }
            if (demand.devices != null)
            {
                foreach (var d in demand.devices)
                {
                    DeviceDemandEntity entity = new DeviceDemandEntity().Build(d);
                    entity.demand_id = demandId;
                    entity.created_at_timestamp = DateTime.Now;
                    await entity.InsertAsync(_context);
                }
            }

            return demandEntity.token;
        }
        
        
        private static string CreateToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            StringBuilder sb = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                while (sb.Length != Constants.DemandTokenLength)
                {
                    var oneByte = new byte[1];
                    rng.GetBytes(oneByte);
                    var randomCharacter = (char)oneByte[0];
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
