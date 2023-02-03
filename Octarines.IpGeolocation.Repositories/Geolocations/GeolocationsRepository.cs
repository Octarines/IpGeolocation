using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Octarines.IpGeolocation.Models.Geolocations;
using Octarines.IpGeolocation.Models.Settings;

namespace Octarines.IpGeolocation.Repositories.Geolocations;

public class GeolocationsRepository: IRepository<Geolocation>
{
    private readonly IMongoCollection<Geolocation> _geolocations;

    public GeolocationsRepository(
        IMongoClient mongoClient,
        IOptions<MongoDbSettings> mongoDbSettings)
    {
        IMongoDatabase database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _geolocations = database.GetCollection<Geolocation>(nameof(Geolocation));
    }

    public async Task<Geolocation> GetAsync(string ipAddress)
    {
        FilterDefinition<Geolocation> filter = Builders<Geolocation>.Filter.Eq(o => o.Ip, ipAddress);
        return await _geolocations.FindAsync(filter).Result.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Geolocation>> GetAsync()
    {
        IAsyncCursor<Geolocation>? results = await _geolocations.FindAsync(FilterDefinition<Geolocation>.Empty);
        return results.ToList();
    }

    public async Task<Geolocation> InsertAsync(Geolocation instance)
    {
        await _geolocations.InsertOneAsync(instance);
        return instance;
    }

    public async Task<Geolocation> UpdateAsync(Geolocation instance)
    {
        FilterDefinition<Geolocation> filter = Builders<Geolocation>.Filter.Eq(o => o.Ip, instance.Ip);
        ReplaceOneResult result = await _geolocations.ReplaceOneAsync(filter, instance);

        return instance;
    }
}