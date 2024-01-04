using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SportDataImport.Enums;

namespace SportDataImport.Mongo;

internal sealed record class Game
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("gameCode")]
    public long? GameCode { get; set; }

    [BsonElement("seasonCode")]
    public string? SeasonCode { get; set; }

    [BsonElement("seasonAlias")]
    public string? SeasonAlias { get; set; }

    [BsonElement("competitionCode")]
    public Competition Competition { get; set; }

    [BsonElement("year")]
    public long? Year { get; set; }

    [BsonElement("startDate")]
    public DateTimeOffset? StartDate { get; set; }

    [BsonElement("group")]
    public Group? Group { get; set; }

    [BsonElement("phaseType")]
    public PhaseType? PhaseType { get; set; }

    [BsonElement("round")]
    public long? Round { get; set; }

    [BsonElement("played")]
    public bool? Played { get; set; }

    [BsonElement("localTimeZone")]
    public long? LocalTimeZone { get; set; }

    [BsonElement("localDate")]
    public DateTimeOffset? LocalDate { get; set; }

    [BsonElement("utcDate")]
    public DateTimeOffset? UtcDate { get; set; }

    [BsonElement("local")]
    public Side? Local { get; set; }

    [BsonElement("road")]
    public Side? Road { get; set; }

    public Game(long? gameCode, string? seasonCode, string? seasonAlias, Competition competition,
        long? year, DateTimeOffset? startDate, Group? group, PhaseType? phaseType, long? round,
        bool? played, long? localTimeZone, DateTimeOffset? localDate, DateTimeOffset? utcDate,
        Side? local, Side? road)
    {
        Id = ObjectId.GenerateNewId();
        GameCode = gameCode;
        SeasonCode = seasonCode;
        SeasonAlias = seasonAlias;
        Competition = competition;
        Year = year;
        StartDate = startDate;
        Group = group;
        PhaseType = phaseType;
        Round = round;
        Played = played;
        LocalTimeZone = localTimeZone;
        LocalDate = localDate;
        UtcDate = utcDate;
        Local = local;
        Road = road;
    }
}

public record class Group(Guid? Id, long? Order, string? Name, string? RawName)
{
    [BsonElement("id")]
    public Guid? Id { get; set; } = Id;

    [BsonElement("order")]
    public long? Order { get; set; } = Order;

    [BsonElement("name")]
    public string? Name { get; set; } = Name;

    [BsonElement("rawName")]
    public string? RawName { get; set; } = RawName;
}

public record class Side(Club? Club, string[]? Last5Form, long? Score, long? StandingsScore)
{
    [BsonElement("club")]
    public Club? Club { get; set; } = Club;

    [BsonElement("last5Form")]
    public string[]? Last5Form { get; set; } = Last5Form;

    [BsonElement("score")]
    public long? Score { get; set; } = Score;

    [BsonElement("standingsScore")]
    public long? StandingsScore { get; set; } = StandingsScore;
}

public record class Club(string? Code, string? Name, string? AbbreviatedName)
{
    [BsonElement("code")]
    public string? Code { get; set; } = Code;

    [BsonElement("name")]
    public string? Name { get; set; } = Name;

    [BsonElement("abbreviatedName")]
    public string? AbbreviatedName { get; set; } = AbbreviatedName;
}

public record class PhaseType(string? Code, string? Alias, string? Name, bool? IsGroupPhase)
{
    [BsonElement("code")]
    public string? Code { get; set; } = Code;

    [BsonElement("alias")]
    public string? Alias { get; set; } = Alias;

    [BsonElement("name")]
    public string? Name { get; set; } = Name;

    [BsonElement("isGroupPhase")]
    public bool? IsGroupPhase { get; set; } = IsGroupPhase;
}

