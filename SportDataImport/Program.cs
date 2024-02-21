using Microsoft.Extensions.DependencyInjection;
using SportDataImport.Interfaces;
using SportDataImport.Jobs;

namespace SportDataImport;

internal class Program
{
    static async Task Main()
    {
        var services = ServiceConfiguration.ConfigureServices();

        var x = services.GetService<IEuroleagueFeaturesService>()!;
        await x!.PrepareFeatureData(Constants.EuroleagueSeasonsWithBetOddsData);

        var scheduleImport = services.GetService<IScheduleImportJob>();
        await scheduleImport!.ImportEuroleagueScheduleAsync();

        var scheduleOutcomeEvaluator = services.GetService<IScheduleOutcomeEvaluatorJob>();
        await scheduleOutcomeEvaluator!.ImportEuroleagueScheduleAsync();
    }
}