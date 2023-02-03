using Octarines.IpGeolocation.Models.Geolocations;
using Octarines.IpGeolocation.Models.ServiceResults;

namespace Octarines.IpGeolocation.Services.Geolocations;

public interface IGeolocationsService
{
    public Task<Result<Geolocation>> GetGeolocationFromProvider(string ipAddress);
    public Task<Result<Geolocation>> GetGeolocation(string ipAddress);
    public Task<Result<IEnumerable<Geolocation>>> GetGeolocations();
}