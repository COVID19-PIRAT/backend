using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pirat.Services.Resource
{
    public static class Constants
    {
        public const int TokenLength = 30;
        //TODO Should we use default values if km is 0 in queries?
        public const int KmDistanceDefaultPersonal = 50;
        public const int KmDistanceDefaultDevice = 50;
        public const int KmDistanceDefaultConsumable = 50;
    }
}
