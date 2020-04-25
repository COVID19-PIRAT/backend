namespace Pirat.Codes
{

    /// <summary>
    /// Use these codes when the server or the database reach a state that should never be reached
    /// </summary>
    public static class FatalCodes
    {

        public const string MoreThanOneOfferFromToken = "9000:MoreThanOneOfferFromToken";
        public const string UpdatesMadeInTooManyRows = "9001:UpdatesMadeInTooManyRows";

        public const string MoreThanOneDemandFromToken = "9003:MoreThanOneDemandFromToken";

    }

    public static class FailureCodes
    {

        public const string InvalidAddress = "1000:InvalidAddress";
        public const string InvalidMail = "1001:InvalidMail";
        public const string InvalidAmountConsumable = "1002:InvalidAmountConsumable";
        public const string InvalidAmountDevice = "1003:InvalidAmountDevice";
        public const string InvalidToken = "1004:InvalidToken";
        public const string InvalidCategoryConsumable = "1005:InvalidCategoryConsumable";
        public const string InvalidCategoryDevice = "1006:InvalidCategoryDevice";
        public const string InvalidReason = "1007:InvalidReason";
        public const string InvalidRegionCode = "1008:InvalidRegionCode";

        public const string InvalidChangeAddress = "1100:InvalidChangeAddress";
        public const string InvalidChangeProvider = "1102:InvalidChangeProvider";
        public const string InvalidChangeConsumable = "1103:InvalidChangeConsumable";
        public const string InvalidChangeDevice = "1104:InvalidChangeDevice";
        public const string InvalidChangePersonal = "1105:InvalidChangePersonal";
        public const string InvalidChangeDemand = "1106:InvalidChangeDemand";

        public const string IncompleteAddress = "2000:IncompleteAddress";
        public const string IncompleteOffer = "2001:IncompleteOffer";
        public const string IncompleteProvider = "2002:IncompleteProvider";
        public const string IncompleteConsumable = "2003:IncompleteConsumable";
        public const string IncompleteDevice = "2004:IncompleteDevice";
        public const string IncompletePersonal = "2005:IncompletePersonal";
        public const string IncompleteDemand = "2006:IncompleteDemand";

        public const string NotFoundAddress = "3000:NotFoundAddress";
        public const string NotFoundOffer = "3001:NotFoundOffer";
        public const string NotFoundProvider = "3002:NotFoundProvider";
        public const string NotFoundConsumable = "3003:NotFoundConsumable";
        public const string NotFoundDevice = "3004:NotFoundDevice";
        public const string NotFoundPersonal = "3005:NotFoundPersonal";
        public const string NotFoundDemand = "3006:NotFoundDemand";

        public const string NotFoundChangeAddress = "3100:NotFoundChangeAddress";
        public const string NotFoundChangeProviderInOffer = "3102:NotFoundChangeProviderInOffer";
        public const string NotFoundChangeConsumable = "3103:NotFoundChangeConsumable";
        public const string NotFoundChangeDevice = "3104:NotFoundChangeDevice";
        public const string NotFoundChangePersonal = "3105:NotFoundChangePersonal";

    }
}
