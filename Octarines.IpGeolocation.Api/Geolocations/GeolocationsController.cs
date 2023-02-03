using Microsoft.AspNetCore.Mvc;
using Octarines.IpGeolocation.Api.Extensions;
using Octarines.IpGeolocation.Models.Geolocations;
using Octarines.IpGeolocation.Models.ServiceResults;
using Octarines.IpGeolocation.Services.Geolocations;

namespace Octarines.IpGeolocation.Api.Geolocations;

[ApiController]
[Route("[controller]")]
public class GeolocationsController : ControllerBase
{
    private readonly IGeolocationsService _geolocationsService;

    public GeolocationsController(IGeolocationsService geolocationsService)
    {
        _geolocationsService = geolocationsService;
    }

    [HttpGet("IpAddress/{ipAddress}")]
    public async Task<IActionResult> GetGeolocationByIp(string ipAddress)
    {
        Result<Geolocation> result = await _geolocationsService.GetGeolocation(ipAddress);

        return this.FromResult(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPersistedGeolocations()
    {
        Result<IEnumerable<Geolocation>> result = await _geolocationsService.GetGeolocations();

        return this.FromResult(result);
    }
}