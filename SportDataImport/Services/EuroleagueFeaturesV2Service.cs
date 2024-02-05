using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SportDataImport.Domain.Enums;
using SportDataImport.Interfaces;
using SportDataImport.Mongo.Entities;
using SportDataImport.Mongo.Interfaces;

namespace SportDataImport.Services;

internal sealed class EuroleagueFeaturesV2Service : IEuroleagueFeaturesService
{
    private readonly IMongoService<EuroleagueFeature> _featuresCollection;
    private readonly IMongoService<EuroleagueFeatureV2> _featuresV2Collection;
    private readonly IMongoService<Game> _gamesCollection;
    private readonly ILogger<EuroleagueFeaturesV2Service> _logger;
    private readonly string _competitionCode;

    public EuroleagueFeaturesV2Service(
        IMongoService<EuroleagueFeature> featuresCollection,
        IMongoService<EuroleagueFeatureV2> featuresV2Collection,
        IMongoService<Game> gamesCollection,
        ILogger<EuroleagueFeaturesV2Service> logger)
    {
        _featuresCollection = featuresCollection;
        _featuresV2Collection = featuresV2Collection;
        _gamesCollection = gamesCollection;
        
        _logger = logger;
        _competitionCode = Competition.Euroleague.ToAbbreviation();
    }

    public async Task PrepareFeatureData()
    {
        var games = await _gamesCollection.GetAll();
        var features = await _featuresCollection.GetAll();

        var featuresV2 = new List<EuroleagueFeatureV2>();
        foreach (var feature in features)
        {
            var gameDate = games.FirstOrDefault(x => x.SeasonCode == feature.SeasonCode && x.Round == feature.Round)?.UtcDate?.UtcDateTime ?? throw new ArgumentNullException(nameof(Game.UtcDate));

            var localTeamGamesPast3Seasons = games.Where(x => IsPrevious3SeasonsForTeam(x, feature.Home, _competitionCode, gameDate));
            var localTeamAgainstRoadTeamPast3Seasons = localTeamGamesPast3Seasons.Where(x => x.Local!.Club!.Code == feature.Road || x.Road!.Club!.Code == feature.Road);
            float localTeamAgainstRoadTeamWinPercentage = (float)localTeamAgainstRoadTeamPast3Seasons.Where(x => x.Local!.Club!.Code == feature.Home ? x.Local.Score > x!.Road!.Score! : x!.Road!.Score! > x.Local.Score).Count() / (float)localTeamAgainstRoadTeamPast3Seasons.Count();

            float roadTeamAgainstLocalTeamWinPercentage = 1f - localTeamAgainstRoadTeamWinPercentage;

            featuresV2.Add(new EuroleagueFeatureV2(localTeamAgainstRoadTeamWinPercentage, roadTeamAgainstLocalTeamWinPercentage, feature));
        }

        var featuresCount = await _featuresV2Collection.Count();

        if (featuresCount > 0)
        {
            await _featuresV2Collection.DeleteBy(x => true);
        }

        await _featuresV2Collection.InsertMany(featuresV2);
    }

    private static bool IsPrevious3SeasonsForTeam(Game game, string clubCode, string competitionCode, DateTime gameDate)
    {
        var currentSeason = $"{competitionCode}{gameDate.Year}";
        var takeSeasonCodes = new[] { $"{competitionCode}{gameDate.Year - 1}", $"{competitionCode}{gameDate.Year - 2}", $"{competitionCode}{gameDate.Year - 3}" };

        return
            (game!.Local!.Club!.Code == clubCode || game!.Road!.Club!.Code == clubCode) &&
            (takeSeasonCodes.Contains(game.SeasonCode) ||
            (game.SeasonCode == currentSeason && game.LocalDate.HasValue && game.LocalDate.Value < gameDate));
    }

}
