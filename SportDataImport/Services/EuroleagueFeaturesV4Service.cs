using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SportDataImport.Domain.Enums;
using SportDataImport.Interfaces;
using SportDataImport.Mongo.Entities;
using SportDataImport.Mongo.Interfaces;

namespace SportDataImport.Services;

internal class EuroleagueFeaturesV4Service : IEuroleagueFeaturesService
{
    private readonly IMongoService<EuroleagueFeatureV2> _featuresV2Collection;
    private readonly IMongoService<EuroleagueFeatureV4> _featuresV4Collection;
    private readonly IMongoService<Game> _gamesCollection;
    private readonly ILogger<EuroleagueFeaturesV4Service> _logger;

    public EuroleagueFeaturesV4Service(
        ILogger<EuroleagueFeaturesV4Service> logger, 
        IMongoService<EuroleagueFeatureV2> featuresV2Collection, 
        IMongoService<EuroleagueFeatureV4> featuresV4Collection, 
        IMongoService<Game> gamesCollection)
    {
        _logger = logger;
        _featuresV2Collection = featuresV2Collection;
        _featuresV4Collection = featuresV4Collection;
        _gamesCollection = gamesCollection;
    }

    public async Task PrepareFeatureData()
    {
        var games = await _gamesCollection.GetAll();
        var featuresV2 = await _featuresV2Collection.GetAll();

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
                        weekType = WeekType.DoubleWeekSecond;
                    }
                    else if (nextGame != null && DatesAreInTheSameWeek(featureGameDate.UtcDateTime, nextGame.UtcDate?.UtcDateTime ??
                        throw new InvalidDataException($"{nameof(Game.UtcDate)} is not defined")))
                    {
                        weekType = WeekType.DoubleWeekFirst;
                    }
                    else
                    {
                        weekType = WeekType.Regular;
                    }
                }
            }

            featuresV4.Add(new EuroleagueFeatureV4(weekType, featureV2));
        }

        var featuresCount = await _featuresV4Collection.Count();
        if (featuresCount > 0)
        {
            await _featuresV4Collection.DeleteBy(x => true);
        }
        await _featuresV4Collection.InsertMany(featuresV4);
    }

    private bool DatesAreInTheSameWeek(DateTime date1, DateTime date2)
    {
        var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
        var d1 = date1.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date1));
        var d2 = date2.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date2));

        return d1 == d2;
    }
}
