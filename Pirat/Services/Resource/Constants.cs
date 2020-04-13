namespace Pirat.Services.Resource
{
    public static class Constants
    {
        public const int OfferTokenLength = 30;
        public const int DemandTokenLength = 32;
        //TODO Should we use default values if km is 0 in queries?
        public const int KmDistanceDefaultPersonal = 50;
        public const int KmDistanceDefaultDevice = 50;
        public const int KmDistanceDefaultConsumable = 50;
    }
}
