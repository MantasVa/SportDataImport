using Microsoft.Extensions.Logging;
using SportDataImport.Domain;
using SportDataImport.Domain.Enums;
using SportDataImport.Interfaces;
using SportDataImport.Mongo.Entities;
using SportDataImport.Mongo.Interfaces;
using System.Globalization;

namespace SportDataImport.Services;

internal class EuroleagueFeaturesV5Service : IEuroleagueFeaturesService
{

    private readonly IMongoService<Game> _gamesCollection;
    private readonly IMongoService<GameOdds> _oddsCollection;
    private readonly IMongoService<Team> _teamsCollection;
    private readonly IMongoService<Book> _booksCollection;
    private readonly IMongoService<EuroleagueFeatureV5> _featuresCollection;
    private readonly ILogger<EuroleagueFeaturesV5Service> _logger;
    private readonly Random _random = new();

    public EuroleagueFeaturesV5Service(
        IMongoService<Game> gamesCollection,
        IMongoService<GameOdds> oddsCollection,
        IMongoService<Team> teamsCollection,
        IMongoService<Book> booksCollection,
        IMongoService<EuroleagueFeatureV5> featuresCollection,
        ILogger<EuroleagueFeaturesV5Service> logger)
    {
        _gamesCollection = gamesCollection;
        _oddsCollection = oddsCollection;
        _teamsCollection = teamsCollection;
        _booksCollection = booksCollection;
        _featuresCollection = featuresCollection;
        _logger = logger;
    }

    public async Task PrepareFeatureData(string[] seasons)
    {
        var finalFourTeams = await _teamsCollection.GetBy(x => x.FinalFourAppearances != null);
        var books = (await _booksCollection.GetOne(x => x.Competition == Competition.Euroleague)).Books;

        var features = new List<EuroleagueFeatureV5>();
        foreach (var season in seasons) 
        {
            var seasonYear = int.Parse(season.Replace(Competition.Euroleague.ToAbbreviation(), string.Empty));
            var odds = await _oddsCollection.GetBy(x => x.SeasonCode == season);
            var games = await _gamesCollection.GetBy(x => x.SeasonCode == season);
            var skippedCounter = 0;
            var featuresCreated = 0;

            foreach (var game in games)
            {
                var homeTeam = game!.Local!.Club!.Code!;
                var awayTeam = game!.Road!.Club!.Code!;

                var gameOdd = odds.FirstOrDefault(x => x.SeasonCode == game.SeasonCode && x.HomeCode == homeTeam && x.AwayCode == awayTeam && x.StartDate.Date == game.LocalDate?.LocalDateTime.Date);

                if (gameOdd == null)
                {
                    _logger.LogWarning("Season {Season} odds are not found for game {Home}:{Away} with start date {StartDate} in round {Round} and game code {Code}", game.SeasonCode, game.Local.Club.Code, game.Road.Club.Code, game.LocalDate?.LocalDateTime, game.Round, game.GameCode);
                    skippedCounter++;
                    continue;
                }
                var gameOdds = gameOdd.Odds.ToDictionary(x => x.Name, x => (HomeOdd: x.HomeOdd.ToProbability(), AwayOdd: x.AwayOdd.ToProbability()));
                var retrievedOdds = books.Where(x => gameOdds.TryGetValue(x, out var y))
                    .Select(x => (BookName: x, gameOdds[x].HomeOdd, gameOdds[x].AwayOdd))
                    .ToList();

                if (!retrievedOdds.Any())
                {
                    _logger.LogError("Cannot pull out odds for game {}:{} in season {} and round {}", homeTeam, awayTeam, season, game.Round);
                    skippedCounter++;
                    continue;
                }

                var odds1 = retrievedOdds[0];
                var odds2 = retrievedOdds.Count >= 2 ? retrievedOdds[1] : retrievedOdds[0];
                var odds3 = retrievedOdds.Count >= 3 ? retrievedOdds[2] : retrievedOdds[0];

                var homeWinPercentage = await GetTeamWinPercentage(homeTeam, game.UtcDate!.Value.UtcDateTime);
                var homeFinalFours = GetTeamPlayedPastFinalFours(finalFourTeams, homeTeam, seasonYear, 3);
                var homeForm = GetTeamPlayForm(games, game, homeTeam, season, game.Round!.Value, homeWinPercentage);

                var awayWinPercentage = await GetTeamWinPercentage(awayTeam, game.UtcDate!.Value.UtcDateTime);
                var awayFinalFours = GetTeamPlayedPastFinalFours(finalFourTeams, awayTeam, seasonYear, 3);
                var awayForm = GetTeamPlayForm(games, game, awayTeam, season, game.Round!.Value, awayWinPercentage);

                var feature = new EuroleagueFeatureV5
                {
                    Season = season,
                    Round = game.Round.Value,
                    BookName1 = odds1.BookName,
                    BookName2 = odds2.BookName,
                    BookName3 = odds3.BookName,
                    InsertedAt = DateTime.UtcNow,

                    Home = homeTeam,
                    HomeLast1 = homeForm[0],
                    HomeLast2 = homeForm[1],
                    HomeLast3 = homeForm[2],
                    HomeLast4 = homeForm[3],
                    HomeLast5 = homeForm[4],
                    HomeIsFinalFour1 = homeFinalFours[0],
                    HomeIsFinalFour2 = homeFinalFours[1],
                    HomeIsFinalFour3 = homeFinalFours[2],
                    HomeWinPercentage = homeWinPercentage,
                    HomeOdds1 = odds1.HomeOdd,
                    HomeOdds2 = odds2.HomeOdd,
                    HomeOdds3 = odds3.HomeOdd,

                    Away = awayTeam,
                    AwayLast1 = awayForm[0],
                    AwayLast2 = awayForm[1],
                    AwayLast3 = awayForm[2],
                    AwayLast4 = awayForm[3],
                    AwayLast5 = awayForm[4],
                    AwayIsFinalFour1 = awayFinalFours[0],
                    AwayIsFinalFour2 = awayFinalFours[1],
                    AwayIsFinalFour3 = awayFinalFours[2],
                    AwayWinPercentage = awayWinPercentage,
                    AwayOdds1 = odds1.AwayOdd,
                    AwayOdds2 = odds2.AwayOdd,
                    AwayOdds3 = odds3.AwayOdd,

                    HomeWin = game.Local.Score > game.Road.Score
                };
                features.Add(feature);
                featuresCreated++;
            }

            _logger.LogInformation("{Counter} games were skipped in season {Season}", skippedCounter, season);
            _logger.LogInformation("{Counter} games were added for season {Season}", featuresCreated, season);
        }

        var existingFeatures = await _featuresCollection.GetAll();
        foreach (var feature in features)
        {
            var dupFeatures = existingFeatures.Where(x => x.Season == feature.Season && x.Round == feature.Round && x.Home == feature.Home && x.Away == feature.Away).Select(x => x.Id).ToHashSet();
            if (dupFeatures.Count != 0)
            {
                await _featuresCollection.DeleteBy(x => dupFeatures.Contains(x.Id));
            };
        }
        await _featuresCollection.InsertMany(features);
    }

    public int[] GetTeamPlayedPastFinalFours(List<Team> finalFourTeams, string teamId, int toYear, int yearsCount)
    {
        var teamFinalFours = finalFourTeams.FirstOrDefault(x => x.Id == teamId)?.FinalFourAppearances;
        var yearsRange = Enumerable.Range(toYear - yearsCount + 1, yearsCount + 1);

        if (teamFinalFours == null)
        {
            return yearsRange.Select(_ => 0).ToArray();
        }

        return yearsRange.Select(year => Convert.ToInt32(teamFinalFours.Contains(year))).ToArray();
    }

    public async Task<float> GetTeamWinPercentage(string teamId, DateTime gameDate)
    {
        var compCode = Competition.Euroleague.ToAbbreviation();
        var currentSeason = $"{compCode}{gameDate.Year}";
        var takeSeasonCodes = new[] { $"{compCode}{gameDate.Year - 1}", $"{compCode}{gameDate.Year - 2}" };

        var games = (await _gamesCollection.GetBy(x => (x.Local.Club.Code == teamId || x.Road.Club.Code == teamId) && 
            (takeSeasonCodes.Contains(x.SeasonCode) || (x.SeasonCode == currentSeason || x.UtcDate < gameDate))))
            .Select(x => x.Local.Club.Code == teamId ? x.Local.Score > x.Road.Score : x.Road.Score > x.Local.Score);

        float teamWinPercentage = games.Where(x => x == true).Count() / (float)games.Count();
        return teamWinPercentage;
    }

    public int[] GetTeamPlayForm(List<Game> games, Game game, string teamId,  string season, long round, float winPercentage)
    {
        var fromRound = Math.Max(1, round - 5);
        var last5Form = games.Where(x => x.Round >= fromRound && x.Round < round && x.SeasonCode == season &&
               (x.Local.Club.Code == teamId || x.Road.Club.Code == teamId))
            .OrderByDescending(x => x.UtcDate)
            .Select(x => x.Local.Club.Code == teamId ? Convert.ToInt32(x.Local.Score > x.Road.Score) : Convert.ToInt32(x.Road.Score > x.Local.Score))
            .ToList();

        if (round > 5 && last5Form.Count < 5 && (Constants.Top16Seasons.Contains(season) || PhaseExtensions.ToPhase(game.PhaseType.Code) == Phase.FinalFour))
        {
            last5Form = games.Where(x => x.Round < round && x.SeasonCode == season &&
               (x.Local.Club.Code == teamId || x.Road.Club.Code == teamId))
            .OrderByDescending(x => x.UtcDate)
            .Take(5)
            .Select(x => x.Local.Club.Code == teamId ? Convert.ToInt32(x.Local.Score > x.Road.Score) : Convert.ToInt32(x.Road.Score > x.Local.Score))
            .ToList();
        }

        ExpectCorrectFormCount(teamId, round, last5Form);
        var teamWins = new[] {
            last5Form.Count > 0 ? last5Form[0] : _random.NextDouble() < winPercentage ? 1 : 0,
            last5Form.Count > 1 ? last5Form[1] : _random.NextDouble() < winPercentage ? 1 : 0,
            last5Form.Count > 2 ? last5Form[2] : _random.NextDouble() < winPercentage ? 1 : 0,
            last5Form.Count > 3 ? last5Form[3] : _random.NextDouble() < winPercentage ? 1 : 0,
            last5Form.Count > 4 ? last5Form[4] : _random.NextDouble() < winPercentage ? 1 : 0
            };

        return teamWins;
    }

    private void ExpectCorrectFormCount(string teamId, long gameRound, List<int> lastForm)
    {
        if (!((gameRound >= 5 && lastForm.Count == 5) || gameRound - 1 == lastForm.Count))
        {
            _logger.LogError("Team {Team} form did not match round {Round} state", teamId, gameRound);
            throw new ArgumentOutOfRangeException(nameof(lastForm), "Last form array does not match round state.");
        }
    }
}
