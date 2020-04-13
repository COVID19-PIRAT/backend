using Pirat.Other;

namespace Pirat.Services.Helper
{
    public enum BoundingBoxType
    {
        Normal = 0,
        WrappsSouthPole = 1,
        WrappsNorthPole = 2,
        WrappsHorizontalNorth = 4,
        WrappsHorizontalSouth = 8,
    }

    public class BoundingBox
    {
        public BoundingBox(Location center, double radius)
        {
            this.Center = center;
            this.Radius = radius;
            SouthWestCorner = center.Move(-radius, -radius);
            SouthEastCorner = center.Move(radius, -radius);
            NorthWestCorner = center.Move(-radius, radius);
            NorthEastCorner = center.Move(radius, radius);

            this.Type = DetermineBoxType();
        }

        public Location Center { get; }
        public double Radius { get; }
        public Location NorthEastCorner { get; }

        public Location NorthWestCorner { get; }

        public Location SouthEastCorner { get; }

        public Location SouthWestCorner { get; }
        public BoundingBoxType Type { get; }

        /// <summary>
        /// Determines how a BoundingBox behaves at the edges.
        /// </summary>
        /// <returns>The type of the bounding box</returns>
        private BoundingBoxType DetermineBoxType()
        {
            var wrappingSouth 
                = SouthEastCorner.Longitude < SouthWestCorner.Longitude;
            var wrappingNorth 
                = NorthEastCorner.Longitude < NorthWestCorner.Longitude;

            var distanceToNorth = Location.NorthPole.Distance(Center);
            var distanceToSouth = Location.SouthPole.Distance(Center);

            var wrappingNorthPole = distanceToNorth < Radius;
            var wrappingSouthPole = distanceToSouth < Radius;

            var type = BoundingBoxType.Normal;
            if (wrappingSouth)
            {
                type |= BoundingBoxType.WrappsHorizontalSouth;
            }

            if (wrappingNorth)
            {
                type |= BoundingBoxType.WrappsHorizontalNorth;
            }

            if (wrappingNorthPole)
            {
                type |= BoundingBoxType.WrappsNorthPole;
            }

            if (wrappingSouthPole)
            {
                type |= BoundingBoxType.WrappsSouthPole;
            }

            return type;
        }

        public bool IsInRadius(Location other)
        {
            NullCheck.ThrowIfNull(other);

            return this.Center.Distance(other) <= this.Radius;
        }
    }
}