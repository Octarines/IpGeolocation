using Microsoft.AspNetCore.Mvc;
using Octarines.IpGeolocation.Api.Geolocations;
using Octarines.IpGeolocation.Models.Geolocations;
using Octarines.IpGeolocation.Models.ServiceResults;
using Octarines.IpGeolocation.Services.Geolocations;

namespace Octarines.IpGeolocaion.Api.Tests.Geolocations;

public class GeolocationsControllerUnitTests
{
    private const string GEOLOCATION_IP = "192.168.0.1";
    
    private readonly Mock<IGeolocationsService> _geolocationsService;
    

    public GeolocationsControllerUnitTests()
    {
        _geolocationsService = new Mock<IGeolocationsService>();
    }
    
    [Fact]
    public async Task GetGeolocationByIp_ValidParameters_ReturnGeolocation()
    {
        //arrange
        Geolocation expectedGeolocation = new Geolocation()
        {
            Ip = GEOLOCATION_IP
        };

        SuccessResult<Geolocation> expectedResult = new SuccessResult<Geolocation>(expectedGeolocation);

        _geolocationsService.Setup(x => x.GetGeolocation(It.IsAny<string>()))
            .ReturnsAsync(expectedResult);

        GeolocationsController controller = new GeolocationsController(_geolocationsService.Object);
        
        //act
        OkObjectResult? result = (await controller.GetGeolocationByIp(GEOLOCATION_IP)) as OkObjectResult;

        //assert
        Assert.NotNull(result);

        Geolocation? geolocation = result.Value as Geolocation;
        Assert.NotNull(geolocation);
        
        Assert.Equal(expectedGeolocation, geolocation);
        
        _geolocationsService.Verify(x =>x.GetGeolocation(
            It.Is<string>(y => string.Equals(GEOLOCATION_IP, y))), Times.Once);
    }
    
    [Fact]
    public async Task GetPersistedGeolocations_NoParameters_ReturnGeolocations()
    {
        //arrange
        Geolocation expectedGeolocation = new Geolocation()
        {
            Ip = GEOLOCATION_IP
        };

        IEnumerable<Geolocation> expectedGeolocations = new List<Geolocation>()
        {
            expectedGeolocation
        };

        SuccessResult<IEnumerable<Geolocation>> expectedResult = new SuccessResult<IEnumerable<Geolocation>>(expectedGeolocations);

        _geolocationsService.Setup(x => x.GetGeolocations())
            .ReturnsAsync(expectedResult);

        GeolocationsController controller = new GeolocationsController(_geolocationsService.Object);
        
        //act
        OkObjectResult? result = (await controller.GetPersistedGeolocations()) as OkObjectResult;

        //assert
        Assert.NotNull(result);

        IEnumerable<Geolocation>? geolocations = result.Value as IEnumerable<Geolocation>;
        Assert.NotNull(geolocations);
        
        Assert.Equal(expectedGeolocations, geolocations);
        
        _geolocationsService.Verify(x =>x.GetGeolocations(), Times.Once);
    }
}