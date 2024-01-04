using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SportDataImport.Enums;
using SportDataImport.Interfaces;
using SportDataImport.Mongo;

namespace SportDataImport.Services;

internal class EuroleagueFeaturesV3Service : IEuroleagueFeaturesService
{
    private readonly ILogger<EuroleagueFeaturesV3Service> _logger;
    private readonly string _competitionCode;

    public EuroleagueFeaturesV3Service(
        ILogger<EuroleagueFeaturesV3Service> logger)
    {
        _logger = logger;
        _competitionCode = Competition.Euroleague.ToAbbreviation();
    }

    public async Task PrepareFeatureData()
    {
        var mongoClient = new MongoClient(Constants.ConnectionString);
        var database = mongoClient.GetDatabase(Constants.DatabaseName);
        var gamesCollection = database.GetCollection<Game>(Constants.GameCollectionName);
        var euroleagueFeaturesV2Collection = database.GetCollection<EuroleagueFeatureV2>(Constants.EuroleagueFeaturesV2CollectionName);

        var games = await gamesCollection.Find(_ => true).ToListAsync();
        var featuresV2 = await euroleagueFeaturesV2Collection.Find(_ => true).ToListAsync();

        var featuresV3 = new List<EuroleagueFeatureV3>();
        foreach (var featureV2 in featuresV2)
        {
            var featureGame = games.Single(x => x.SeasonCode == featureV2.SeasonCode && x.GameCode == featureV2.Round);

            var homeGames = games
                .Where(x => IsPrevious3SeasonForTeam(x, featureV2.Home, _competitionCode, featureGame.StartDate!.Value.DateTime))
                .Where(x => x!.Local!.Club!.Code == featureV2.Home);
            var homeLocalWinsPercentage = (float)homeGames.Where(x => x!.Local!.Score > x!.Road!.Score).Count() / (float)homeGames.Count();

            var awayGames = games
                .Where(x => IsPrevious3SeasonForTeam(x, featureV2.Road, _competitionCode, featureGame.StartDate!.Value.DateTime))
                .Where(x => x!.Road!.Club!.Code == featureV2.Road);
            var roadAwayWinsPercentage = (float)awayGames.Where(x => x!.Road!.Score > x!.Local!.Score).Count() / (float)awayGames.Count();
            
            featuresV3.Add(new EuroleagueFeatureV3(homeLocalWinsPercentage, roadAwayWinsPercentage, featureV2));
        }

        var featuresV3Collection = database.GetCollection<EuroleagueFeatureV3>(Constants.EuroleagueFeaturesV3CollectionName);
        var featuresCount = await featuresV3Collection.CountDocumentsAsync(x => true);

        if (featuresCount > 0)
        {
            featuresV3Collection.DeleteMany(x => true);
        }
        await featuresV3Collection.InsertManyAsync(featuresV3);
    }

    private static bool IsPrevious3SeasonForTeam(Game game, string clubCode, string competitionCode, DateTime gameDate)
    {
        var currentSeason = $"{competitionCode}{gameDate.Year}";
        var takeSeasonCodes = new[] { $"{competitionCode}{gameDate.Year - 1}", $"{competitionCode}{gameDate.Year - 2}", $"{competitionCode}{gameDate.Year - 3}" };

        return
            (game!.Local!.Club!.Code == clubCode || game!.Road!.Club!.Code == clubCode) &&
            (takeSeasonCodes.Contains(game.SeasonCode) ||
            (game.SeasonCode == currentSeason && game.LocalDate.HasValue && game.LocalDate.Value < gameDate));
    }
}
