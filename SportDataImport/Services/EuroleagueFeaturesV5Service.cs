using Microsoft.Extensions.Logging;
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

    public async Task PrepareFeatureData()
    {
        var finalFourTeams = await _teamsCollection.GetBy(x => x.FinalFourAppearances != null);
        var books = (await _booksCollection.GetOne(x => x.Competition == Competition.Euroleague)).Books;

        var features = new List<EuroleagueFeatureV5>();
        foreach (var season in Constants.EuroleagueSeasonsWithBetOddsData) 
        {
            var seasonYear = int.Parse(season.Replace(Competition.Euroleague.ToAbbreviation(), string.Empty));
            var odds = await _oddsCollection.GetBy(x => x.SeasonCode == season);
            var games = await _gamesCollection.GetBy(x => x.SeasonCode == season);

            foreach (var game in games)
            {
                var homeTeam = game!.Local!.Club!.Code!;
                var awayTeam = game!.Road!.Club!.Code!;

                var gameOdd = odds.FirstOrDefault(x => x.SeasonCode == game.SeasonCode && x.HomeCode == homeTeam && x.AwayCode == awayTeam && x.StartDate == game.LocalDate?.LocalDateTime);

                if (gameOdd == null)
                {
                    _logger.LogWarning("Season {Season} odds are not found for game {Home}:{Away} with start date {StartDate}", game.SeasonCode, game.Local.Club.Code, game.Road.Club.Code, game.StartDate.ToString());
                    continue;
                }
                var gameOdds = gameOdd.Odds.ToDictionary(x => x.Name, x => (x.HomeOdd, x.AwayOdd));
                var retrievedOdds = books.Where(x => gameOdds.TryGetValue(x, out var y) && 
                float.TryParse(y.HomeOdd, NumberStyles.Any, CultureInfo.InvariantCulture, out _) && 
                float.TryParse(y.AwayOdd, NumberStyles.Any, CultureInfo.InvariantCulture, out _)).Select(x => (BookName: x, HomeOdd: float.Parse(gameOdds[x].HomeOdd, NumberStyles.Any, CultureInfo.InvariantCulture), AwayOdd: float.Parse(gameOdds[x].AwayOdd, NumberStyles.Any, CultureInfo.InvariantCulture))).FirstOrDefault();

                if (retrievedOdds == default)
                {
                    _logger.LogError("Cannot pull out odds for game {}:{} in season {} and round {}", homeTeam, awayTeam, season, game.Round);
                    continue;
                }

                var homeWinPercentage = await GetTeamWinPercentage(homeTeam, game.UtcDate!.Value.UtcDateTime);
                var homeFinalFours = GetTeamPlayedPastFinalFours(finalFourTeams, homeTeam, seasonYear, 3);
                var homeForm = GetTeamPlayForm(games, homeTeam, season, game.Round!.Value, homeWinPercentage);

                var awayWinPercentage = await GetTeamWinPercentage(awayTeam, game.UtcDate!.Value.UtcDateTime);
                var awayFinalFours = GetTeamPlayedPastFinalFours(finalFourTeams, awayTeam, seasonYear, 3);
                var awayForm = GetTeamPlayForm(games, awayTeam, season, game.Round!.Value, awayWinPercentage);

                var feature = new EuroleagueFeatureV5
                {
                    SeasonCode = season,
                    Round = game.Round.Value,
                    BookName = retrievedOdds.BookName,

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
                    HomeOdds = retrievedOdds.HomeOdd,

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
                    AwayOdds = retrievedOdds.AwayOdd,

                    HomeWin = game.Local.Score > game.Road.Score
                };
                features.Add(feature);
            }
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

    public int[] GetTeamPlayForm(List<Game> games, string teamId,  string season, long round, float winPercentage)
    {
        var fromRound = Math.Max(1, round - 5);
        var last5Form = games.Where(x => x.Round >= fromRound && x.Round < round && x.SeasonCode == season &&
               (x.Local.Club.Code == teamId || x.Road.Club.Code == teamId))
            .OrderByDescending(x => x.UtcDate)
            .Select(x => x.Local.Club.Code == teamId ? Convert.ToInt32(x.Local.Score > x.Road.Score) : Convert.ToInt32(x.Road.Score > x.Local.Score))
            .ToList();

        ExpectCorrectFormCount(round, last5Form);
        var teamWins = new[] {
            last5Form.Count > 0 ? last5Form[0] : _random.NextDouble() < winPercentage ? 1 : 0,
            last5Form.Count > 1 ? last5Form[1] : _random.NextDouble() < winPercentage ? 1 : 0,
            last5Form.Count > 2 ? last5Form[2] : _random.NextDouble() < winPercentage ? 1 : 0,
            last5Form.Count > 3 ? last5Form[3] : _random.NextDouble() < winPercentage ? 1 : 0,
            last5Form.Count > 4 ? last5Form[4] : _random.NextDouble() < winPercentage ? 1 : 0
            };

        return teamWins;
    }

    private static void ExpectCorrectFormCount(long gameRound, List<int> lastForm)
    {
        if (!((gameRound >= 5 && lastForm.Count == 5) || gameRound - 1 == lastForm.Count))
        {
            throw new ArgumentOutOfRangeException(nameof(lastForm), "Last form array does not match round state.");
        }
    }
}
