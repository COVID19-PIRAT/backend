using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pirat.Codes;
using Pirat.Exceptions;
using Pirat.Model;
using Pirat.Model.Entity.Resource.Common;

namespace Pirat.Services
{
    public interface IAddressMaker {

        public void SetCoordinates(AddressEntity address);

    }
}