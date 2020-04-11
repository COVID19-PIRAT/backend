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
            public const string INVALID_TOKEN = "1004:InvalidToken";
            public const string INVALID_CATEGORY_CONSUMABLE = "1005:InvalidCategoryConsumable";
            public const string INVALID_CATEGORY_DEVICE = "1006:InvalidCategoryDevice";
            public const string INVALID_REASON = "1007:InvalidReason";

            public const string INVALID_CHANGE_ADDRESS = "1100:InvalidChangeAddress";
            public const string INVALID_CHANGE_PROVIDER = "1102:InvalidChangeProvider";
            public const string INVALID_CHANGE_CONSUMABLE = "1103:InvalidChangeConsumable";
            public const string INVALID_CHANGE_DEVICE = "1104:InvalidChangeDevice";
            public const string INVALID_CHANGE_PERSONAL = "1105:InvalidChangePersonal";
            public const string INVALID_CHANGE_DEMAND = "1106:InvalidChangeDemand";

            public const string INCOMPLETE_ADDRESS = "2000:IncompleteAddress";
            public const string INCOMPLETE_OFFER = "2001:IncompleteOffer";
            public const string INCOMPLETE_PROVIDER = "2002:IncompleteProvider";
            public const string INCOMPLETE_CONSUMABLE = "2003:IncompleteConsumable";
            public const string INCOMPLETE_DEVICE = "2004:IncompleteDevice";
            public const string INCOMPLETE_PERSONAL = "2005:IncompletePersonal";
            public const string INCOMPLETE_DEMAND = "2006:IncompleteDemand";

            public const string NOTFOUND_ADDRESS = "3000:NotFoundAddress";
            public const string NOTFOUND_OFFER = "3001:NotFoundOffer";
            public const string NOTFOUND_PROVIDER = "3002:NotFoundProvider";
            public const string NOTFOUND_CONSUMABLE = "3003:NotFoundConsumable";
            public const string NOTFOUND_DEVICE = "3004:NotFoundDevice";
            public const string NOTFOUND_PERSONAL = "3005:NotFoundPersonal";
            public const string NOTFOUND_DEMAND = "3006:NotFoundDemand";

            public const string NOTFOUND_CHANGE_ADDRESS = "3100:NotFoundChangeAddress";
            public const string NOTFOUND_CHANGE_PROVIDER_IN_OFFER = "3102:NotFoundChangeProviderInOffer";
            public const string NOTFOUND_CHANGE_CONSUMABLE = "3103:NotFoundChangeConsumable";
            public const string NOTFOUND_CHANGE_DEVICE = "3104:NotFoundChangeDevice";
            public const string NOTFOUND_CHANGE_PERSONAL = "3105:NotFoundChangePersonal";

        }

        /// <summary>
        /// Use these codes when the server or the database reach a state that should never be reached
        /// </summary>
        public static class FatalCodes
        {

            public const string MORE_THAN_ONE_OFFER_FROM_TOKEN = "9000:MoreThanOneOfferFromToken";
            public const string UPDATES_MADE_IN_TOO_MANY_ROWS = "9001:UpdatesMadeInTooManyRows";
            public const string RESOURCE_WITHOUT_RELATED_ADDRESS = "9002:ResourceWithoutRelatedAddress";
            public const string MORE_THAN_ONE_DEMAND_FROM_TOKEN = "9003:MoreThanOneDemandFromToken";

        }
    }
}
