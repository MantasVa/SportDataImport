using Microsoft.Extensions.DependencyInjection;
using SportDataImport.Import;
using SportDataImport.Jobs;

namespace SportDataImport;

internal class Program
{
    static async Task Main()
    {
        var services = ServiceConfiguration.ConfigureServices();

        var scheduleImport = services.GetService<IScheduleImportJob>();
        await scheduleImport!.ImportEuroleagueScheduleAsync();

        var scheduleOutcomeEvaluator = services.GetService<IScheduleOutcomeEvaluatorJob>();
        await scheduleOutcomeEvaluator!.ImportEuroleagueScheduleAsync();
    }
}