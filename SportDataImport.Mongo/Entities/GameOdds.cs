using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SportDataImport.Domain.Enums;

namespace SportDataImport.Mongo.Entities;

// TODO: ADD collection name
public sealed record class GameOdds
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("season")]
    public string? SeasonCode { get; set; }

    [BsonElement("competitionCode")]
    public Competition Competition { get; set; }

    [BsonElement("game_date")]
    public DateTime StartDate { get; set; }

    [BsonElement("home_score")]
    public long? HomeScore { get; set; }

    [BsonElement("home_code")]
    public string HomeCode { get; set; }

    [BsonElement("home")]
    public string HomeTeamName { get; set; }

    [BsonElement("away_score")]
    public long? AwayScore { get; set; }

    [BsonElement("away_code")]
    public string AwayCode { get; set; }

    [BsonElement("away")]
    public string AwayTeamName { get; set; }

    [BsonElement("odds")]
    public Book[] Odds { get; set; }

    [BsonElement("link")]
    public string OddsLink { get; set; }
}

public sealed record class Book
{
    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("home")]
    public string HomeOdd { get; set; }

    [BsonElement("away")]
    public string AwayOdd { get; set; }
}