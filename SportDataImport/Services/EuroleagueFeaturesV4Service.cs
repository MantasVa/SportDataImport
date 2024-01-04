using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SportDataImport.Enums;
using SportDataImport.Interfaces;
using SportDataImport.Mongo;

namespace SportDataImport.Services;

internal class EuroleagueFeaturesV4Service : IEuroleagueFeaturesService
{

    private readonly ILogger<EuroleagueFeaturesV4Service> _logger;

    public EuroleagueFeaturesV4Service(
        ILogger<EuroleagueFeaturesV4Service> logger)
    {
        _logger = logger;
    }

    public async Task PrepareFeatureData()
    {
        var mongoClient = new MongoClient(Constants.ConnectionString);
        var database = mongoClient.GetDatabase(Constants.DatabaseName);
        var gamesCollection = database.GetCollection<Game>(Constants.GameCollectionName);
        var euroleagueFeaturesV2Collection = database.GetCollection<EuroleagueFeatureV2>(Constants.EuroleagueFeaturesV2CollectionName);

        var games = await gamesCollection.Find(_ => true).ToListAsync();
        var featuresV2 = await euroleagueFeaturesV2Collection.Find(_ => true).ToListAsync();

        var featuresV4 = new List<EuroleagueFeatureV4>();
        foreach (var featureV2 in featuresV2)
        {
            WeekType weekType;
            if (featureV2.Round == 1)
            {
                weekType = WeekType.Regular;
            }
            else
            {

                var featureGame = games.Single(x => x.SeasonCode == featureV2.SeasonCode && x.Round == featureV2.Round
                                    && x.Local!.Club!.Code == featureV2.Home && x.Road!.Club!.Code == featureV2.Road);
                var featureGameDate = featureGame.UtcDate ?? throw new InvalidDataException($"{nameof(Game.UtcDate)} is not defined");

                var previousGame = games.FirstOrDefault(x => x.SeasonCode == featureV2.SeasonCode && x.Round == featureV2.Round - 1
                                        && (x.Local!.Club!.Code == featureV2.Home || x.Road!.Club!.Code == featureV2.Home));

                if (previousGame == null)
                {
                    _logger.LogWarning("Previous game is null for season {SeasonCode}, game code {GameCode} and round {Round}",
                        featureGame.SeasonCode, featureGame.GameCode, featureGame.Round);
                    weekType = WeekType.Regular;
                }
                else
                {
                    var previousGameDate = previousGame.UtcDate ?? throw new InvalidDataException($"{nameof(Game.UtcDate)} is not defined");

                    var nextGame = games.FirstOrDefault(x => x.SeasonCode == featureV2.SeasonCode && x.Round == featureV2.Round + 1
                                                && (x.Local!.Club!.Code == featureV2.Home || x.Road!.Club!.Code == featureV2.Home));

                    if (DatesAreInTheSameWeek(previousGameDate.UtcDateTime, featureGameDate.UtcDateTime))
                    {
                        weekType = WeekType.DoubleWeek2;
                    }
                    else if (nextGame != null && DatesAreInTheSameWeek(featureGameDate.UtcDateTime, nextGame.UtcDate?.UtcDateTime ??
                        throw new InvalidDataException($"{nameof(Game.UtcDate)} is not defined")))
                    {
                        weekType = WeekType.DoubleWeek1;
                    }
                    else
                    {
                        weekType = WeekType.Regular;
                    }
                }
            }

            featuresV4.Add(new EuroleagueFeatureV4(weekType, featureV2));
        }

        var featuresV4Collection = database.GetCollection<EuroleagueFeatureV4>(Constants.EuroleagueFeaturesV4CollectionName);
        var featuresCount = await featuresV4Collection.CountDocumentsAsync(x => true);

        if (featuresCount > 0)
        {
            featuresV4Collection.DeleteMany(x => true);
        }
        await featuresV4Collection.InsertManyAsync(featuresV4);
    }

    private bool DatesAreInTheSameWeek(DateTime date1, DateTime date2)
    {
        var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
        var d1 = date1.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date1));
        var d2 = date2.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date2));

        return d1 == d2;
    }
}
