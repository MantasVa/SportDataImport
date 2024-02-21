using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SportDataImport.Clients;
using SportDataImport.Domain;
using SportDataImport.Mongo.Entities;
using SportDataImport.Mongo.Interfaces;

namespace SportDataImport.Import;

internal interface IGamesImport
{
    Task ImportEuroleagueGamesAsync(string[] seasonCodes);
}

internal sealed class GamesImport : IGamesImport
{
    private readonly IMongoService<Game> _gamesCollection;
    private readonly IEuroleagueClient _euroleagueClient;
    private readonly ILogger _logger;

    public GamesImport(
        IEuroleagueClient euroleagueClient,
        ILogger<GamesImport> logger,
        IMongoService<Game> gamesCollection)
    {
        _euroleagueClient = euroleagueClient;
        _logger = logger;
        _gamesCollection = gamesCollection;
    }

    public async Task ImportEuroleagueGamesAsync(string[] seasonCodes)
    {
        var currentEuroleagueSeasonCode = EuroleagueHelper.GetCurrentEuroleagueSeasonCode();

        var games = new List<Game>();
        foreach (var seasonCode in seasonCodes)
        {
            foreach (var gameCode in Enumerable.Range(1, 400))
            {
                var (isSuccess, game) = await _euroleagueClient.GetGame(seasonCode, gameCode);

                if (!isSuccess)
                {
                    _logger.LogWarning("Game call is not successful in season {SeasonCode} game code {GameCode}", seasonCode, gameCode);
                    break;
                }

                if (game == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(game));
                }

                if (seasonCode == currentEuroleagueSeasonCode && game.UtcDate >= DateTime.UtcNow)
                {
                    break;
                }

                _logger.LogInformation("Game added in Season {SeasonCode} game code {GameCode}", seasonCode, gameCode);

                games.Add(game.ToGame());
            }
        }
        await _gamesCollection.InsertMany(games);

        _logger.LogInformation("Euroleague game import complete. Imported {Count} games", games.Count);
    }
}

public partial class GameDto
{
    [JsonProperty("gameCode", NullValueHandling = NullValueHandling.Ignore)]
    public long? GameCode { get; set; }

    [JsonProperty("season", NullValueHandling = NullValueHandling.Ignore)]
    public SeasonDto? Season { get; set; }

    [JsonProperty("group", NullValueHandling = NullValueHandling.Ignore)]
    public GroupDto? Group { get; set; }

    [JsonProperty("phaseType", NullValueHandling = NullValueHandling.Ignore)]
    public PhaseTypeDto? PhaseType { get; set; }

    [JsonProperty("round", NullValueHandling = NullValueHandling.Ignore)]
    public long? Round { get; set; }

    [JsonProperty("roundAlias", NullValueHandling = NullValueHandling.Ignore)]
    public string? RoundAlias { get; set; }

    [JsonProperty("roundName", NullValueHandling = NullValueHandling.Ignore)]
    public string? RoundName { get; set; }

    [JsonProperty("played", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Played { get; set; }

    [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? Date { get; set; }

    [JsonProperty("confirmedDate", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ConfirmedDate { get; set; }

    [JsonProperty("confirmedHour", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ConfirmedHour { get; set; }

    [JsonProperty("localTimeZone", NullValueHandling = NullValueHandling.Ignore)]
    public long? LocalTimeZone { get; set; }

    [JsonProperty("localDate", NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? LocalDate { get; set; }

    [JsonProperty("utcDate", NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? UtcDate { get; set; }

    [JsonProperty("local", NullValueHandling = NullValueHandling.Ignore)]
    public SideDto? Local { get; set; }

    [JsonProperty("road", NullValueHandling = NullValueHandling.Ignore)]
    public SideDto? Road { get; set; }

    [JsonProperty("localLast5Form", NullValueHandling = NullValueHandling.Ignore)]
    public string[]? LocalLast5Form { get; set; }

    [JsonProperty("roadLast5Form", NullValueHandling = NullValueHandling.Ignore)]
    public string[]? RoadLast5Form { get; set; }
}

public partial class GroupDto
{
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public Guid? Id { get; set; }

    [JsonProperty("order", NullValueHandling = NullValueHandling.Ignore)]
    public long? Order { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty("rawName", NullValueHandling = NullValueHandling.Ignore)]
    public string? RawName { get; set; }
}

public partial class SideDto
{
    [JsonProperty("club", NullValueHandling = NullValueHandling.Ignore)]
    public ClubDto? Club { get; set; }

    [JsonProperty("score", NullValueHandling = NullValueHandling.Ignore)]
    public long? Score { get; set; }

    [JsonProperty("standingsScore", NullValueHandling = NullValueHandling.Ignore)]
    public long? StandingsScore { get; set; }
}

public partial class ClubDto
{
    [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
    public string? Code { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty("abbreviatedName", NullValueHandling = NullValueHandling.Ignore)]
    public string? AbbreviatedName { get; set; }

    [JsonProperty("editorialName", NullValueHandling = NullValueHandling.Ignore)]
    public string? EditorialName { get; set; }

    [JsonProperty("tvCode", NullValueHandling = NullValueHandling.Ignore)]
    public string? TvCode { get; set; }

    [JsonProperty("isVirtual", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsVirtual { get; set; }

    [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore)]
    public ImagesDto? Images { get; set; }
}

public partial class ImagesDto
{
    [JsonProperty("crest", NullValueHandling = NullValueHandling.Ignore)]
    public string? Crest { get; set; }
}

public partial class PhaseTypeDto
{
    [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
    public string? Code { get; set; }

    [JsonProperty("alias", NullValueHandling = NullValueHandling.Ignore)]
    public string? Alias { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty("isGroupPhase", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsGroupPhase { get; set; }
}

public partial class SeasonDto
{
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
    public string? Code { get; set; }

    [JsonProperty("alias", NullValueHandling = NullValueHandling.Ignore)]
    public string? Alias { get; set; }

    [JsonProperty("competitionCode", NullValueHandling = NullValueHandling.Ignore)]
    public string? CompetitionCode { get; set; }

    [JsonProperty("year", NullValueHandling = NullValueHandling.Ignore)]
    public long? Year { get; set; }

    [JsonProperty("startDate", NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? StartDate { get; set; }
}
