using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SportDataImport.Domain.Enums;
using SportDataImport.Interfaces;
using SportDataImport.Mongo.Entities;
using SportDataImport.Mongo.Interfaces;

namespace SportDataImport.Services;

internal class EuroleagueFeaturesV3Service : IEuroleagueFeaturesService
{
    private readonly IMongoService<EuroleagueFeatureV2> _featuresV2Collection;
    private readonly IMongoService<EuroleagueFeatureV3> _featuresV3Collection;
    private readonly IMongoService<Game> _gamesCollection;
    private readonly ILogger<EuroleagueFeaturesV3Service> _logger;
    private readonly string _competitionCode;

    public EuroleagueFeaturesV3Service(
        ILogger<EuroleagueFeaturesV3Service> logger,
        IMongoService<EuroleagueFeatureV2> featuresV2Collection,
        IMongoService<EuroleagueFeatureV3> featuresV3Collection,
        IMongoService<Game> gamesCollection)
    {
        _logger = logger;
        _competitionCode = Competition.Euroleague.ToAbbreviation();
        _featuresV2Collection = featuresV2Collection;
        _featuresV3Collection = featuresV3Collection;
        _gamesCollection = gamesCollection;
    }

    public async Task PrepareFeatureData(string[] seasons)
    {
        var games = await _gamesCollection.GetAll();
        var featuresV2 = await _featuresV2Collection.GetAll();

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

        var featuresCount = await _featuresV3Collection.Count();

        if (featuresCount > 0)
        {
            await _featuresV3Collection.DeleteBy(x => true);
        }
        await _featuresV3Collection.InsertMany(featuresV3);
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
