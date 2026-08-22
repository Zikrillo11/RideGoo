using RideGoo.Domain.Exceptions;

namespace RideGoo.Domain.ValueObjects;

public sealed class GeoLocation : IEquatable<GeoLocation>
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    private GeoLocation() { }   // EF Core uchun

    private GeoLocation(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static GeoLocation Create(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            throw new DomainException("Kenglik (latitude) -90 dan 90 gacha bo'lishi kerak.");

        if (longitude is < -180 or > 180)
            throw new DomainException("Uzunlik (longitude) -180 dan 180 gacha bo'lishi kerak.");

        return new GeoLocation(latitude, longitude);
    }

    public double DistanceToKm(GeoLocation other)
    {
        const double earthRadiusKm = 6371;

        var dLat = ToRadians(other.Latitude - Latitude);
        var dLon = ToRadians(other.Longitude - Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;

    public bool Equals(GeoLocation? other) =>
        other is not null && Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);

    public override bool Equals(object? obj) => Equals(obj as GeoLocation);
    public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
}