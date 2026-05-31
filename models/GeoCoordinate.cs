using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectFIN.models
{
    public struct GeoCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoCoordinate(double lat, double lng)
        {
            Latitude = lat;
            Longitude = lng;
        }
        public double DistanceTo(GeoCoordinate other)
        {
            double r = 6371000;
            double phi1 = Latitude * Math.PI / 180;
            double phi2 = other.Latitude * Math.PI / 180;
            double deltaPhi = (other.Latitude - Latitude) * Math.PI / 180;
            double deltaLambda = (other.Longitude - Longitude) * Math.PI / 180;

            double a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2) +
                       Math.Cos(phi1) * Math.Cos(phi2) *
                       Math.Sin(deltaLambda / 2) * Math.Sin(deltaLambda / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return r * c;
        }
    }
}