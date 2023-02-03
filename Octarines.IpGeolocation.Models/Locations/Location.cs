using System.Text.Json.Serialization;
using Octarines.IpGeolocation.Models.Languages;

namespace Octarines.IpGeolocation.Models.Locations;

public class Location
{
    [JsonPropertyName("geoname_id")]
    public int GeonameId { get; set; }

    [JsonPropertyName("capital")]
    public string Capital { get; set; }

    [JsonPropertyName("languages")]
    public List<Language> Languages { get; set; }

    [JsonPropertyName("country_flag")]
    public string CountryFlag { get; set; }

    [JsonPropertyName("country_flag_emoji")]
    public string CountryFlagEmoji { get; set; }

    [JsonPropertyName("country_flag_emoji_unicode")]
    public string CountryFlagEmojiUnicode { get; set; }

    [JsonPropertyName("calling_code")]
    public string CallingCode { get; set; }

    [JsonPropertyName("is_eu")]
    public bool IsEu { get; set; }
}