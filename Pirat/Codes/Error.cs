namespace Pirat.Codes
{
    public class Error
    {
        public static class ErrorCodes
        {

            public const string INVALID_ADDRESS = "1000:InvalidAddress";
            public const string INVALID_MAIL = "1001:InvalidMail";
            public const string INVALID_AMOUNT_CONSUMABLE = "1002:InvalidAmountConsumable";
            public const string INVALID_AMOUNT_DEVICE = "1003:InvalidAmountDevice";

            public const string INCOMPLETE_ADDRESS = "2000:IncompleteAddress";
            public const string INCOMPLETE_OFFER = "2001:IncompleteOffer";
            public const string INCOMPLETE_PROVIDER = "2002:IncompleteProvider";
            public const string INCOMPLETE_CONSUMABLE = "2003:IncompleteConsumable";
            public const string INCOMPLETE_DEVICE = "2004:IncompleteDevice";
            public const string INCOMPLETE_PERSONAL = "2005:IncompletePersonal";
            public const string INCOMPLETE_TOKEN = "2006:IncompleteToken";

            public const string NOTFOUND_ADDRESS = "3000:NotFoundAddress";
            public const string NOTFOUND_OFFER = "3001:NotFoundOffer";
            public const string NOTFOUND_PROVIDER = "3002:NotFoundProvider";
            public const string NOTFOUND_CONSUMABLE = "3003:NotFoundConsumable";
            public const string NOTFOUND_DEVICE = "3004:NotFoundDevice";
            public const string NOTFOUND_PERSONAL = "3005:NotFoundPersonal";

        }

        public static class FatalCodes
        {

            public const string MORE_THAN_ONE_OFFER_FROM_TOKEN = "9000:MoreThanOneOfferFromToken";
    
        }
    }
}
