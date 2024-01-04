using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SportDataImport.Enums;
using SportDataImport.Interfaces;
using SportDataImport.Mongo;

namespace SportDataImport.Services;

internal sealed class EuroleagueFeaturesV2Service : IEuroleagueFeaturesService
{
    private readonly ILogger<EuroleagueFeaturesService> _logger;
    private readonly string _competitionCode;

    public EuroleagueFeaturesV2Service(
        ILogger<EuroleagueFeaturesService> logger)
    {
        _logger = logger;
        _competitionCode = Competition.Euroleague.ToAbbreviation();
    }

    public async Task PrepareFeatureData()
    {
        var mongoClient = new MongoClient(Constants.ConnectionString);
        var database = mongoClient.GetDatabase(Constants.DatabaseName);
        var gamesCollection = database.GetCollection<Game>(Constants.GameCollectionName);
        var euroleagueFeaturesCollection = database.GetCollection<EuroleagueFeature>(Constants.EuroleagueFeaturesCollectionName);

        var games = await gamesCollection.Find(x => true).ToListAsync();
        var features = await euroleagueFeaturesCollection.Find(x => true).ToListAsync();

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

        var featuresV2Collection = database.GetCollection<EuroleagueFeatureV2>(Constants.EuroleagueFeaturesV2CollectionName);
        var featuresCount = await featuresV2Collection.CountDocumentsAsync(x => true);

        if (featuresCount > 0)
        {
            featuresV2Collection.DeleteMany(x => true);
        }

        await featuresV2Collection.InsertManyAsync(featuresV2);
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
