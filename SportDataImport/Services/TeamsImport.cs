using System.Text.Json;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using SportDataImport.Clients;
using SportDataImport.Mongo.Entities;
using SportDataImport.Mongo.Interfaces;

namespace SportDataImport.Import;

internal interface ITeamsImport
{
    Task ImportAsync();
}

internal sealed class TeamsImport : ITeamsImport
{
    private readonly IMongoService<Team> _teamsCollection;
    private readonly IEuroleagueClient _euroleagueClient;
    private readonly ILogger _logger;

    public TeamsImport(
        IEuroleagueClient euroleagueClient,
        ILogger<TeamsImport> logger,
        IMongoService<Team> teamCollection)
    {
        _teamsCollection = teamCollection;
        _euroleagueClient = euroleagueClient;
        _logger = logger;
    }

    public async Task ImportAsync()
    {
        var (isSuccess, teams) = await _euroleagueClient.GetTeams();

        if (!isSuccess)
        {
            _logger.LogError("Teams endpoint call was unsuccessful");
            return;
        }

        if (teams == null)
        {
            throw new ArgumentOutOfRangeException(nameof(teams));
        }

        var teamsToInsert = teams.Select(x => x.ToTeam()).ToList();
        await _teamsCollection.InsertMany(teamsToInsert);

        _logger.LogInformation("{Count} teams were imported", teams.Length);
    }
}

public class TeamDto
{
    [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
    public string? Code { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty("alias", NullValueHandling = NullValueHandling.Ignore)]
    public string? Alias { get; set; }

    [JsonProperty("isVirtual", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsVirtual { get; set; }

    [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
    public CountryDto? Country { get; set; }

    [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
    public string? Address { get; set; }

    [JsonProperty("website", NullValueHandling = NullValueHandling.Ignore)]
    public string? Website { get; set; }

    [JsonProperty("ticketsUrl", NullValueHandling = NullValueHandling.Ignore)]
    public string? TicketsUrl { get; set; }

    [JsonProperty("twitterAccount", NullValueHandling = NullValueHandling.Ignore)]
    public string? TwitterAccount { get; set; }

    [JsonProperty("instagramAccount", NullValueHandling = NullValueHandling.Ignore)]
    public string? InstagramAccount { get; set; }

    [JsonProperty("facebookAccount", NullValueHandling = NullValueHandling.Ignore)]
    public string? FacebookAccount { get; set; }

    [JsonProperty("venue", NullValueHandling = NullValueHandling.Ignore)]
    public VenueDto? Venue { get; set; }

    [JsonProperty("venueBackup")]
    public string? VenueBackup { get; set; }

    [JsonProperty("nationalCompetitionCode")]
    public string? NationalCompetitionCode { get; set; }

    [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
    public string? City { get; set; }

    [JsonProperty("president", NullValueHandling = NullValueHandling.Ignore)]
    public string? President { get; set; }

    [JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
    public string? Phone { get; set; }

    [JsonProperty("fax", NullValueHandling = NullValueHandling.Ignore)]
    public string? Fax { get; set; }

    [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore)]
    public ImageDto? Images { get; set; }
}

public class CountryDto
{
    [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
    public string? Code { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }
}

public class ImageDto
{
    [JsonProperty("crest", NullValueHandling = NullValueHandling.Ignore)]
    public string? Crest { get; set; }
}

public class VenueDto
{
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string? Name { get; set; }

    [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
    public string? Code { get; set; }

    [JsonProperty("capacity", NullValueHandling = NullValueHandling.Ignore)]
    public long? Capacity { get; set; }

    [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
    public string? Address { get; set; }

    [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore)]
    public VenueImagesDto? Images { get; set; }

    [JsonProperty("active", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Active { get; set; }

    [JsonProperty("notes", NullValueHandling = NullValueHandling.Ignore)]
    public string? Notes { get; set; }
}

public class VenueImagesDto
{
}
