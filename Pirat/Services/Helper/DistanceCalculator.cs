using System;

namespace Pirat.Services.Helper
{
    public static class DistanceCalculator
    {
        public static double ComputeDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            //made a short sketch on paper and just got the formula ;)
            var latitudeRadian = ConvertDegreesToRadians(latitude2 - latitude1);
            var longitudeRadian = ConvertDegreesToRadians(longitude2 - longitude1);

            var a = Math.Sin(latitudeRadian / 2) * Math.Sin(latitudeRadian / 2) +
                    Math.Cos(ConvertDegreesToRadians(latitude1)) *
                    Math.Cos(ConvertDegreesToRadians(latitude2)) * Math.Sin(longitudeRadian / 2) *
                    Math.Sin(longitudeRadian / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = Location.EarthRadius * c;
            return d;
        }

        public static double ComputeDistance(
            decimal latitude1,
            decimal longitude1,
            decimal latitude2,
            decimal longitude2)
            => ComputeDistance(
                decimal.ToDouble(latitude1),
                decimal.ToDouble(longitude1),
                decimal.ToDouble(latitude2),
                decimal.ToDouble(longitude2)
            );

        private static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }
    }
}