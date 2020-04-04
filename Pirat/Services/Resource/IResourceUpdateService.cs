using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pirat.Model;

namespace Pirat.Services.Resource
{
    public interface IResourceUpdateService
    {

        public Task<string> insert(Offer offer);

        public Task<string> delete(string link);
    }
}
