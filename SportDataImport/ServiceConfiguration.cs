using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SportDataImport.Clients;
using SportDataImport.Import;
using SportDataImport.Jobs;

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
            .AddSingleton<IScheduleImportJob, ScheduleImportJob>()
            .AddSingleton<IScheduleImportJob, ScheduleImportJob>()
            .AddSingleton<IScheduleOutcomeEvaluatorJob, ScheduleOutcomeEvaluatorJob>()
            .BuildServiceProvider();

        return serviceProvider;
    }
}
