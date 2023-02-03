using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Moq.Protected;
using Octarines.IpGeolocation.Models.Geolocations;
using Octarines.IpGeolocation.Models.ServiceResults;
using Octarines.IpGeolocation.Models.Settings;
using Octarines.IpGeolocation.Repositories;
using Octarines.IpGeolocation.Services.Geolocations;

namespace Octarines.IpGeolocation.Services.Tests.Geolocations;

public class GeolocationsServiceUnitTests
{
    private const string GEOLOCATION_IP = "192.168.0.1";

    private const string SERVICE_URL = "https://www.someserviceurl.com";
    private const string SERVICE_KEY = "someservicekey";

    private const string EXECEPTION = "sometestexceptionmessage";
    
    private readonly Mock<HttpMessageHandler> _httpMessageHandler;
    private readonly Mock<IRepository<Geolocation>> _geolocationsRepository;
    private readonly Mock<IOptions<IpGeolocationServiceSettings>> _ipGeolocationServiceSettings;

    public GeolocationsServiceUnitTests()
    {
        _httpMessageHandler = new Mock<HttpMessageHandler>();
        _geolocationsRepository = new Mock<IRepository<Geolocation>>();
        
        _ipGeolocationServiceSettings = new Mock<IOptions<IpGeolocationServiceSettings>>();
        
        _ipGeolocationServiceSettings.Setup(x => x.Value)
            .Returns(new IpGeolocationServiceSettings()
            {
                Url = SERVICE_URL,
                Key = SERVICE_KEY
            });

        _geolocationsRepository = new Mock<IRepository<Geolocation>>();
    }

    [Fact]
    public async Task GetGeolocationFromProvider_ValidParameter_ReturnGeolocation()
    {
        //arrange
        Geolocation expectedGeolocation = new Geolocation()
        {
            Ip = GEOLOCATION_IP
        };

        HttpResponseMessage expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedGeolocation)
        };
        
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);
        
        HttpClient httpClient = new HttpClient(_httpMessageHandler.Object);

        GeolocationsService service = new GeolocationsService(
            httpClient,
            _geolocationsRepository.Object,
            _ipGeolocationServiceSettings.Object
            );
        
        //act
        Result<Geolocation> result = await service.GetGeolocationFromProvider(GEOLOCATION_IP);

        //assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors);
        Assert.Equivalent(expectedGeolocation, result.Value);
        
        Assert.Equal(ResultType.Success, result.ResultType);
        
        _httpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetGeolocationFromProvider_ValidParameterNonSuccess_ReturnErrorResult()
    {
        //arrange
        HttpResponseMessage expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        };
        
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);

        string expectedError =
            $"Non-success status (InternalServerError) returned from GET request to '{SERVICE_URL}' for Ip Address '{GEOLOCATION_IP}'";
        
        HttpClient httpClient = new HttpClient(_httpMessageHandler.Object);

        GeolocationsService service = new GeolocationsService(
            httpClient,
            _geolocationsRepository.Object,
            _ipGeolocationServiceSettings.Object
        );
        
        //act
        Result<Geolocation> result = await service.GetGeolocationFromProvider(GEOLOCATION_IP);

        //assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors);
        Assert.Single(result.Errors);
        Assert.Equal(expectedError, result.Errors.FirstOrDefault());
        Assert.Equal(ResultType.Unexpected, result.ResultType);
        
        
        _httpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetGeolocationFromProvider_ValidParameterNullGeolocation_ReturnErrorResult()
    {
        //arrange
        Geolocation expectedGeolocation = null;

        HttpResponseMessage expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedGeolocation)
        };
        
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);

        string expectedError =
            $"Null value returned from GET request to '{SERVICE_URL}' for Ip Address '{GEOLOCATION_IP}'";
        
        HttpClient httpClient = new HttpClient(_httpMessageHandler.Object);

        GeolocationsService service = new GeolocationsService(
            httpClient,
            _geolocationsRepository.Object,
            _ipGeolocationServiceSettings.Object
        );
        
        //act
        Result<Geolocation> result = await service.GetGeolocationFromProvider(GEOLOCATION_IP);

        //assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors);
        Assert.Single(result.Errors);
        Assert.Equal(expectedError, result.Errors.FirstOrDefault());
        Assert.Equal(ResultType.Unexpected, result.ResultType);

        _httpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>());
    }
    
    [Fact]
    public async Task GetGeolocationFromProvider_ValidParameterHttpClientException_ReturnErrorResult()
    {
        //arrange
        Exception expectedException = new Exception(EXECEPTION);
        
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(expectedException);

        string expectedError =
            $"Unexpected error getting {nameof(Geolocation)} from service provider {SERVICE_URL} for ip address '{GEOLOCATION_IP}'";
        
        HttpClient httpClient = new HttpClient(_httpMessageHandler.Object);

        GeolocationsService service = new GeolocationsService(
            httpClient,
            _geolocationsRepository.Object,
            _ipGeolocationServiceSettings.Object
        );
        
        //act
        Result<Geolocation> result = await service.GetGeolocationFromProvider(GEOLOCATION_IP);

        //assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors);
        Assert.NotEmpty(result.Errors);

        Assert.Equal(expectedError, result.Errors.FirstOrDefault());
        Assert.Equal(EXECEPTION, result.Errors.Skip(1).First());
        
        Assert.Equal(ResultType.Unexpected, result.ResultType);

        _httpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetGeolocations_NoParameters_ReturnsGeolocations()
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

        _geolocationsRepository.Setup(x => x.GetAsync())
            .ReturnsAsync(expectedGeolocations);

        HttpClient httpClient = new HttpClient(_httpMessageHandler.Object);
        
        GeolocationsService service = new GeolocationsService(
            httpClient,
            _geolocationsRepository.Object,
            _ipGeolocationServiceSettings.Object
        );
        
        //act
        Result<IEnumerable<Geolocation>> result = await service.GetGeolocations();

        //assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors);
        Assert.Equal(expectedGeolocations, result.Value);
        
        Assert.Equal(ResultType.Success, result.ResultType);

        _geolocationsRepository.Verify(x => x.GetAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetGeolocations_NoParametersRepositoryException_ReturnErrorResult()
    {
        //arrange
        Exception expectedException = new Exception(EXECEPTION);
        
        _geolocationsRepository.Setup(x => x.GetAsync())
            .ThrowsAsync(expectedException);

        HttpClient httpClient = new HttpClient(_httpMessageHandler.Object);
        
        string expectedError =
            $"Database error retrieving {nameof(Geolocation)}s'";
        
        GeolocationsService service = new GeolocationsService(
            httpClient,
            _geolocationsRepository.Object,
            _ipGeolocationServiceSettings.Object
        );
        
        //act
        Result<IEnumerable<Geolocation>> result = await service.GetGeolocations();

        //assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors);
        Assert.NotEmpty(result.Errors);

        Assert.Equal(expectedError, result.Errors.FirstOrDefault());
        Assert.Equal(EXECEPTION, result.Errors.Skip(1).First());
        
        Assert.Equal(ResultType.Unexpected, result.ResultType);

        _geolocationsRepository.Verify(x => x.GetAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetGeolocation_ValidParameter_ReturnGeolocationFromProviderAndPersist()
    {
        //arrange
        Geolocation expectedGeolocation = new Geolocation()
        {
            Ip = GEOLOCATION_IP
        };

        HttpResponseMessage expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedGeolocation)
        };
        
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);
        
        HttpClient httpClient = new HttpClient(_httpMessageHandler.Object);

        GeolocationsService service = new GeolocationsService(
            httpClient,
            _geolocationsRepository.Object,
            _ipGeolocationServiceSettings.Object
        );
        
        //act
        Result<Geolocation> result = await service.GetGeolocation(GEOLOCATION_IP);

        //assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors);
        Assert.Equivalent(expectedGeolocation, result.Value);
        
        Assert.Equal(ResultType.Success, result.ResultType);
        
        _httpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>());
        
        _geolocationsRepository.Verify(x => x.InsertAsync(
            It.Is<Geolocation>(y => y.Ip == expectedGeolocation.Ip)), Times.Once);
    }
    
    [Fact]
    public async Task GetGeolocation_ValidParameterTwoCallsCachedValue_ReturnGeolocationProviderAndRepositoryCalledOnceOnly()
    {
        //arrange
        Geolocation expectedGeolocation = new Geolocation()
        {
            Ip = GEOLOCATION_IP
        };

        HttpResponseMessage expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedGeolocation)
        };
        
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);
        
        HttpClient httpClient = new HttpClient(_httpMessageHandler.Object);

        GeolocationsService service = new GeolocationsService(
            httpClient,
            _geolocationsRepository.Object,
            _ipGeolocationServiceSettings.Object
        );
        
        //act
        Result<Geolocation> _ = await service.GetGeolocation(GEOLOCATION_IP);
        Result<Geolocation> result = await service.GetGeolocation(GEOLOCATION_IP);

        //assert
        Assert.NotNull(result);
        Assert.False(result.HasErrors);
        Assert.Equivalent(expectedGeolocation, result.Value);
        
        Assert.Equal(ResultType.Success, result.ResultType);
        
        _httpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>());
        
        _geolocationsRepository.Verify(x => x.InsertAsync(
            It.Is<Geolocation>(y => y.Ip == expectedGeolocation.Ip)), Times.Once);
    }
    
    [Fact]
    public async Task GetGeolocation_ValidParameterRepositoryException_ReturnErrorResult()
    {
        //arrange
        Exception expectedException = new Exception(EXECEPTION);
        
        _geolocationsRepository.Setup(x => x.InsertAsync(It.IsAny<Geolocation>()))
            .ThrowsAsync(expectedException);
        
        string expectedError =
            $"Database error persisting new {nameof(Geolocation)} record with ip address '{GEOLOCATION_IP}'";
        
        Geolocation expectedGeolocation = new Geolocation()
        {
            Ip = GEOLOCATION_IP
        };

        HttpResponseMessage expectedResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedGeolocation)
        };
        
        _httpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);
        
        HttpClient httpClient = new HttpClient(_httpMessageHandler.Object);

        GeolocationsService service = new GeolocationsService(
            httpClient,
            _geolocationsRepository.Object,
            _ipGeolocationServiceSettings.Object
        );
        
        //act
        Result<Geolocation> result = await service.GetGeolocation(GEOLOCATION_IP);

        //assert
        Assert.NotNull(result);
        Assert.True(result.HasErrors);
        Assert.NotEmpty(result.Errors);

        Assert.Equal(expectedError, result.Errors.FirstOrDefault());
        Assert.Equal(EXECEPTION, result.Errors.Skip(1).First());
        
        Assert.Equal(ResultType.Unexpected, result.ResultType);
        
        _httpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
            ItExpr.IsAny<CancellationToken>());
        
        _geolocationsRepository.Verify(x => x.InsertAsync(
            It.Is<Geolocation>(y => y.Ip == expectedGeolocation.Ip)), Times.Once);
    }
}