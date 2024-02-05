using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SportDataImport.Domain.Enums;
using SportDataImport.Interfaces;
using SportDataImport.Mongo.Entities;
using SportDataImport.Mongo.Interfaces;

namespace SportDataImport.Services;

public sealed class EuroleagueFeaturesService : IEuroleagueFeaturesService
{
    private const int StartSeasonYear = 2003;

    private readonly IMongoService<EuroleagueFeature> _featuresCollection;
    private readonly IMongoService<Game> _gamesCollection;
    private readonly IMongoService<Team> _teamCollection;
    private readonly ILogger<EuroleagueFeaturesService> _logger;
    private readonly string _competitionCode;

    public EuroleagueFeaturesService(
        IMongoService<EuroleagueFeature> featuresCollection,
        IMongoService<Game> gamesCollection,
        IMongoService<Team> teamCollection,
        ILogger<EuroleagueFeaturesService> logger)
    {
        _featuresCollection = featuresCollection;
        _gamesCollection = gamesCollection;
        _teamCollection = teamCollection;
        _logger = logger;
        _competitionCode = Competition.Euroleague.ToAbbreviation();
    }

    public async Task PrepareFeatureData()
    {
        var currentEuroleagueSeasonCode = EuroleagueHelper.GetCurrentEuroleagueSeasonCode();
        var finalFourTeams = await _teamCollection.GetBy(x => x.FinalFourAppearances != null);

        var features = new List<EuroleagueFeature>();
        var games = await _gamesCollection.GetAll();

        // Set data from 2003 season to have all the data.
        var toYear = int.Parse(currentEuroleagueSeasonCode.Replace(Competition.Euroleague.ToAbbreviation(), string.Empty));
        foreach (var year in Enumerable.Range(StartSeasonYear, toYear - StartSeasonYear))
        {
            var seasonGames = games.Where(x => x.SeasonCode == $"{_competitionCode}{year}").ToList();
            foreach (var game in seasonGames)
            {
                if (game!.Local!.Score == 0 && game!.Road!.Score == 0)
                {
                    continue;
                }
                var gameDate = game.UtcDate.HasValue ? game.UtcDate.Value.DateTime : new DateTime(year, 1, 1);

                // Home
                var homeTeam = game!.Local!.Club!.Code;
                var homeGames = games.Where(x => IsPrevious3SeasonsForTeam(x, homeTeam!, _competitionCode, gameDate));
                var home = homeGames.Select(x => x!.Local!.Club!.Code == homeTeam ? x.Local.Score > x!.Road!.Score! : x!.Road!.Score! > x.Local.Score);
                float homeTeamWinPercentage = (float)home.Where(x => x == true).Count() / (float)homeGames.Count();

                var homeFinalFours = finalFourTeams.FirstOrDefault(x => x.Id == homeTeam)?.FinalFourAppearances;
                var homeFinalFour1 = homeFinalFours != null && homeFinalFours.Contains(year) ? 1 : 0;
                var homeFinalFour2 = homeFinalFours != null && homeFinalFours.Contains(year - 1) ? 1 : 0;
                var homeFinalFour3 = homeFinalFours != null && homeFinalFours.Contains(year - 2) ? 1 : 0;

                // Road
                var roadTeam = game!.Road!.Club!.Code;
                var roadGames = games.Where(x => IsPrevious3SeasonsForTeam(x, roadTeam!, _competitionCode, gameDate));
                var road = roadGames.Select(x => x!.Local!.Club!.Code == roadTeam ? x.Local.Score > x!.Road!.Score! : x!.Road!.Score! > x.Local.Score);
                float roadTeamWinPercentage = (float)road.Where(x => x == true).Count() / (float)roadGames.Count();

                var roadFinalFours = finalFourTeams.FirstOrDefault(x => x.Id == roadTeam)?.FinalFourAppearances;
                var roadFinalFour1 = roadFinalFours != null && roadFinalFours.Contains(year) ? 1 : 0;
                var roadFinalFour2 = roadFinalFours != null && roadFinalFours.Contains(year - 1) ? 1 : 0;
                var roadFinalFour3 = roadFinalFours != null && roadFinalFours.Contains(year - 2) ? 1 : 0;

                int? homeLast1 = null, homeLast2 = null, homeLast3 = null, homeLast4 = null, roadLast1 = null, roadLast2 = null, roadLast3 = null, roadLast4 = null;
                if (game.Round.HasValue && game.Round.Value > 1)
                {
                    ExpectCorrectFormCount(game.Round.Value, game!.Local!.Last5Form!);
                    switch (game.Round.Value)
                    {
                        case 2:
                            homeLast1 = ToInt(game!.Local!.Last5Form![0]);

                            roadLast1 = ToInt(game!.Road!.Last5Form![0]);
                            break;
                        case 3:
                            homeLast2 = ToInt(game!.Local!.Last5Form![0]);
                            homeLast1 = ToInt(game!.Local!.Last5Form![1]);

                            roadLast2 = ToInt(game!.Road!.Last5Form![0]);
                            roadLast1 = ToInt(game!.Road!.Last5Form![1]);
                            break;
                        case 4:
                            homeLast3 = ToInt(game!.Local!.Last5Form![0]);
                            homeLast2 = ToInt(game!.Local!.Last5Form![1]);
                            homeLast1 = ToInt(game!.Local!.Last5Form![2]);

                            roadLast3 = ToInt(game!.Road!.Last5Form![0]);
                            roadLast2 = ToInt(game!.Road!.Last5Form![1]);
                            roadLast1 = ToInt(game!.Road!.Last5Form![2]);
                            break;
                        default:
                            homeLast4 = ToInt(game!.Local!.Last5Form![0]);
                            homeLast3 = ToInt(game!.Local!.Last5Form![1]);
                            homeLast2 = ToInt(game!.Local!.Last5Form![2]);
                            homeLast1 = ToInt(game!.Local!.Last5Form![3]);

                            roadLast4 = ToInt(game!.Road!.Last5Form![0]);
                            roadLast3 = ToInt(game!.Road!.Last5Form![1]);
                            roadLast2 = ToInt(game!.Road!.Last5Form![2]);
                            roadLast1 = ToInt(game!.Road!.Last5Form![3]);
                            break;
                    }
                }

                bool homeWin = game.Local.Score > game.Road.Score;

                features.Add(new EuroleagueFeature
                {
                    SeasonCode = game.SeasonCode!,
                    Round = game.Round!.Value,
                    Home = game.Local.Club.Code!,
                    HomeLast1 = homeLast1,
                    HomeLast2 = homeLast2,
                    HomeLast3 = homeLast3,
                    HomeLast4 = homeLast4,
                    HomeIsFinalFour1 = homeFinalFour1,
                    HomeIsFinalFour2 = homeFinalFour2,
                    HomeIsFinalFour3 = homeFinalFour3,
                    HomeWinPercentage = homeTeamWinPercentage,

                    Road = game.Road.Club.Code!,
                    RoadLast1 = roadLast1,
                    RoadLast2 = roadLast2,
                    RoadLast3 = roadLast3,
                    RoadLast4 = roadLast4,
                    RoadIsFinalFour1 = roadFinalFour1,
                    RoadIsFinalFour2 = roadFinalFour2,
                    RoadIsFinalFour3 = roadFinalFour3,
                    RoadWinPercentage = roadTeamWinPercentage,

                    HomeWin = homeWin
                });

                _logger.LogInformation("Game added {SeasonCode} {LocalTeam} {LocalScore}:{RoadScore} {RoadTeam}", game.SeasonCode, homeTeam, game.Local.Score, game.Road.Score, roadTeam);
            }
        }

        var featuresCount = await _featuresCollection.Count();
        if (featuresCount > 0)
        {
            await _featuresCollection.DeleteBy(x => true);
        }

        await _featuresCollection.InsertMany(features);
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

    private static int ToInt(string s) => s.ToLowerInvariant() == "w" ? 1 : 0;

    private static void ExpectCorrectFormCount(long gameRound, string[] lastForm)
    {
        if (!(gameRound >= 5 && lastForm.Length == 5 || gameRound == lastForm.Length))
        {
            throw new ArgumentOutOfRangeException(nameof(lastForm), "Last form array does not match round state.");
        }
    }
}
