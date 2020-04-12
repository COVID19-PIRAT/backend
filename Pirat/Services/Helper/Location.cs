using System;
using System.Diagnostics.CodeAnalysis;
using static System.Math;

namespace Pirat.Services.Helper
{
    /// <summary>
    /// This class represents coordinations on a globe with latitude and
    /// longitude in degree.
    /// </summary>
    public struct Location : IEquatable<Location>
    {
        /// <summary>
        /// The radius of the Earth in kilometers
        /// </summary>
        public const double EarthRadius = 6371.0;

        private const double FullCircelDegree = 360.0;
        private const double HalfCircelDegree = FullCircelDegree / 2.0;
        private const double QuaterCircelDegree = HalfCircelDegree / 2.0;

        private const double RadianToDegreeFactor = HalfCircelDegree / PI;

        public Location(double latitude, double longitude)
        {
            this.Longitude = longitude;
            this.Latitude = latitude;
        }

        public double Latitude { get; }

        public double LatitudeInRadian => Latitude / RadianToDegreeFactor;

        public double Longitude { get; }

        public double LongitudeInRadian => Longitude / RadianToDegreeFactor;

        /// <summary>
        /// Creates a new <see cref="Location"/> that is
        /// <paramref name="distance"/> kilometer moved to the north. If
        /// <paramref name="distance"/> is negativ the new
        /// <see cref="Location"/> will be south. If the distance is
        /// greater/lower as the distance to the pole in which the
        /// <see cref="Location"/> has to move it will move beyond the
        /// respective pole.
        /// </summary>
        /// <param name="distance">the distance in kilometer</param>
        /// <returns>the new <see cref="Location"/></returns>
        public Location MoveVerticly(double distance)
        {
            var newLatitude = Latitude + (distance / EarthRadius * RadianToDegreeFactor);
            var newLongitude = Longitude;
            var wrapps = false;
            if (newLatitude > QuaterCircelDegree) // goes beyond north pole
            {
                newLatitude = HalfCircelDegree - newLatitude;
                wrapps = true;
            }
            else if (newLatitude < -QuaterCircelDegree) // goes beyond south pole
            {
                newLatitude = -HalfCircelDegree - newLatitude;
                wrapps = true;
            }

            if (wrapps)
            {
                if (newLongitude == 0.0)
                {
                    newLongitude = HalfCircelDegree;
                }
                else
                {
                    newLongitude -= (Sign(newLongitude) * HalfCircelDegree);
                }
            }

            return new Location(newLatitude, newLongitude);
        }

        /// <summary>
        /// Creates a new <see cref="Location"/> that is
        /// <paramref name="distance"/> kilometer to the east from the current
        /// <see cref="Location"/>. If the <paramref name="distance"/> is
        /// negativ the position will be moved to the west.
        /// </summary>
        /// <param name="distance">the distance in kilometer</param>
        /// <returns>the new <see cref="Location"/></returns>
        public Location MoveHorizontal(double distance)
        {
            if (this.Latitude == QuaterCircelDegree || this.Latitude == -QuaterCircelDegree)
            {
                return this;
            }
            else
            {
                var newLongitude = Longitude + (distance / EarthRadius * RadianToDegreeFactor / Cos(Latitude / RadianToDegreeFactor));
                if (newLongitude <= -HalfCircelDegree)
                {
                    newLongitude = HalfCircelDegree - (newLongitude + HalfCircelDegree);
                }
                else if (newLongitude > HalfCircelDegree)
                {
                    newLongitude -= FullCircelDegree;
                }

                return new Location(this.Latitude, newLongitude);
            }
        }

        public Location Move(double horizontal, double vertical)
            => this.MoveVerticly(vertical).MoveHorizontal(horizontal);

        #region common overrides

        #region Equality

        public override bool Equals(object obj)
        {
            if (obj is Location other)
            {
                return this.Latitude == other.Latitude
                    && this.Longitude == other.Longitude;
            }
            else
            {
                return false;
            }
        }

        public bool Equals([AllowNull] Location other)
            => this.Equals(obj: other);

        public static bool operator ==(Location left, Location right)
            => left.Equals(right);

        public static bool operator !=(Location left, Location right)
            => !(left == right);

        #endregion Equality

        public override int GetHashCode()
            => (119 * this.Longitude.GetHashCode())
                + (107 * this.Latitude.GetHashCode());

        public override string ToString()
            => $"{{ lat = {Latitude}, long = {Longitude} }}";

        #endregion common overrides
    }
}