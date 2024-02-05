using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SportDataImport.Domain.Enums;
using SportDataImport.Interfaces;
using SportDataImport.Mongo;

namespace SportDataImport.Services;

internal class EuroleagueFeaturesV5Service : IEuroleagueFeaturesService
{

    private readonly ILogger<EuroleagueFeaturesV5Service> _logger;

    public EuroleagueFeaturesV5Service(
        ILogger<EuroleagueFeaturesV5Service> logger)
    {
        _logger = logger;
    }

    public async Task PrepareFeatureData()
    {

    }
}
