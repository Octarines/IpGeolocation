using MongoDB.Driver;
using Octarines.IpGeolocation.Models.Geolocations;
using Octarines.IpGeolocation.Models.Settings;
using Octarines.IpGeolocation.Repositories;
using Octarines.IpGeolocation.Repositories.Geolocations;
using Octarines.IpGeolocation.Services.Geolocations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<MongoDbSettings>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection(nameof(MongoDbSettings)).Bind(settings);
    });
builder.Services.AddOptions<IpGeolocationServiceSettings>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection(nameof(IpGeolocationServiceSettings)).Bind(settings);
    });

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IMongoClient>(u => new MongoClient(builder.Configuration["MongoDbSettings:ConnectionString"]));

builder.Services.AddTransient<IRepository<Geolocation>, GeolocationsRepository>();
builder.Services.AddSingleton<IGeolocationsService, GeolocationsService>();

builder.Services.AddControllers();
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();