using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SportDataImport.Clients;
using SportDataImport.Import;
using SportDataImport.Interfaces;
using SportDataImport.Jobs;
using SportDataImport.Mongo.Entities;
using SportDataImport.Mongo.Interfaces;
using SportDataImport.Mongo.Services;
using SportDataImport.Services;

namespace SportDataImport;

internal static class ServiceConfiguration
{
    internal static ServiceProvider ConfigureServices()
    {
        using IHost host = Host.CreateApplicationBuilder().Build();
        IConfiguration configuration = host.Services.GetRequiredService<IConfiguration>();

        // Set up Serilog for JSON logging to a file
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        var serviceProvider = new ServiceCollection()
            .AddLogging((loggingBuilder) => loggingBuilder.AddSerilog())
            .AddSingleton<IEuroleagueClient, EuroleagueClient>()
            .AddSingleton<IGamesImport, GamesImport>()
            .AddSingleton<ITeamsImport, TeamsImport>()
            .AddSingleton<IEuroleagueFeaturesService, EuroleagueFeaturesV5Service>()
            .AddSingleton<IScheduleImportJob, ScheduleImportJob>()
            .AddSingleton<IScheduleOutcomeEvaluatorJob, ScheduleOutcomeEvaluatorJob>()
            .AddSingleton<IMongoService<EuroleagueFeature>, MongoService<EuroleagueFeature>>()
            .AddSingleton<IMongoService<EuroleagueFeatureV2>, MongoService<EuroleagueFeatureV2>>()
            .AddSingleton<IMongoService<EuroleagueFeatureV3>, MongoService<EuroleagueFeatureV3>>()
            .AddSingleton<IMongoService<EuroleagueFeatureV4>, MongoService<EuroleagueFeatureV4>>()
            .AddSingleton<IMongoService<EuroleagueFeatureV5>, MongoService<EuroleagueFeatureV5>>()
            .AddSingleton<IMongoService<Game>, MongoService<Game>>()
            .AddSingleton<IMongoService<GameOdds>, MongoService<GameOdds>>()
            .AddSingleton<IMongoService<ScheduledEvent>, MongoService<ScheduledEvent>>()
            .AddSingleton<IMongoService<Team>, MongoService<Team>>()
            .AddSingleton<IMongoService<Book>, MongoService<Book>>()
            .BuildServiceProvider();

        return serviceProvider;
    }
}
