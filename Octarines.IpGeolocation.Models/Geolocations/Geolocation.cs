using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Octarines.IpGeolocation.Models.Locations;

namespace Octarines.IpGeolocation.Models.Geolocations;

public class Geolocation
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [JsonPropertyName("ip")]
    public string Ip { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("continent_code")]
    public string ContinentCode { get; set; }

    [JsonPropertyName("continent_name")]
    public string ContinentName { get; set; }

    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; }

    [JsonPropertyName("country_name")]
    public string CountryName { get; set; }

    [JsonPropertyName("region_code")]
    public string RegionCode { get; set; }

    [JsonPropertyName("region_name")]
    public string RegionName { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("zip")]
    public string Zip { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("location")]
    public Location Location { get; set; }
}