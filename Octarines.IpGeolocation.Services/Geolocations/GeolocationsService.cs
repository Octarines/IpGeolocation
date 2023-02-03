using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Octarines.IpGeolocation.Models.Geolocations;
using Octarines.IpGeolocation.Models.ServiceResults;
using Octarines.IpGeolocation.Models.Settings;
using Octarines.IpGeolocation.Repositories;

namespace Octarines.IpGeolocation.Services.Geolocations;

public class GeolocationsService: IGeolocationsService
{
    private readonly HttpClient _httpClient;
    private readonly IpGeolocationServiceSettings _ipGeolocationServiceSettings;
    private readonly Dictionary<string, Geolocation> _geolocationCache;
    private readonly IRepository<Geolocation> _geolocationsRepository;

    public GeolocationsService(
        HttpClient httpClient,
        IRepository<Geolocation> geolocationsRepository,
        IOptions<IpGeolocationServiceSettings> ipGeolocationServiceSettings)
    {
        _httpClient = httpClient;
        _ipGeolocationServiceSettings = ipGeolocationServiceSettings.Value;
        _geolocationCache = new Dictionary<string, Geolocation>();
        _geolocationsRepository = geolocationsRepository;
    }

    public async Task<Result<Geolocation>> GetGeolocation(string ipAddress)
    {
        if (_geolocationCache.ContainsKey(ipAddress))
        {
            return new SuccessResult<Geolocation>(_geolocationCache[ipAddress]);
        }
        
        Result<Geolocation> providerResult = await GetGeolocationFromProvider(ipAddress);
        
        if(providerResult.HasErrors)
        {
            return providerResult;
        }

        _geolocationCache[ipAddress] = providerResult.Value;

        try
        {
            await _geolocationsRepository.InsertAsync(providerResult.Value);
        }
        catch (Exception exception)
        {
            _geolocationCache.Remove(ipAddress);
            return new UnexpectedResult<Geolocation>(new List<string>()
            {
                $"Database error persisting new {nameof(Geolocation)} record with ip address '{ipAddress}'",
                exception.Message
            });
        }

        return providerResult;
    }

    public async Task<Result<IEnumerable<Geolocation>>> GetGeolocations()
    {
        try
        {
            IEnumerable<Geolocation> geolocations = await _geolocationsRepository.GetAsync();
            return new SuccessResult<IEnumerable<Geolocation>>(geolocations);
        }
        catch (Exception exception)
        {
            return new UnexpectedResult<IEnumerable<Geolocation>>(new List<string>()
            {
                $"Database error retrieving {nameof(Geolocation)}s'",
                exception.Message
            });
        }
    }

    public async Task<Result<Geolocation>> GetGeolocationFromProvider(string ipAddress)
    {
        try
        {
            string requestUrl =
                $"{_ipGeolocationServiceSettings.Url}/{ipAddress}?access_key={_ipGeolocationServiceSettings.Key}&format=1";

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                return new UnexpectedResult<Geolocation>(
                    $"Non-success status ({response.StatusCode}) returned from GET request to '{_ipGeolocationServiceSettings.Url}' for Ip Address '{ipAddress}'");
            }

            Geolocation? geolocation = await response.Content.ReadFromJsonAsync<Geolocation>();

            if (geolocation != null)
            {
                return new SuccessResult<Geolocation>(geolocation);
            }
            else
            {
                // TODO handle the fact the returned value was null.
                return new UnexpectedResult<Geolocation>(
                    $"Null value returned from GET request to '{_ipGeolocationServiceSettings.Url}' for Ip Address '{ipAddress}'");
            }
        }
        catch (Exception exception)
        {
            return new UnexpectedResult<Geolocation>(new List<string>()
            {
                $"Unexpected error getting {nameof(Geolocation)} from service provider {_ipGeolocationServiceSettings.Url} for ip address '{ipAddress}'",
                exception.Message
            });
        }
        
    }
}