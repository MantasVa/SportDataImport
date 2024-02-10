using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SportDataImport.Domain.Enums;
using SportDataImport.Mongo.Attributes;

namespace SportDataImport.Mongo.Entities;

[BsonCollectionName(Constants.GameOddsCollectionName)]
public sealed record class GameOdds
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("season")]
    public string? SeasonCode { get; set; }

    [BsonElement("sport")]
    public string? Sport { get; set; }

    [BsonElement("tournament")]
    public string? Tournament { get; set; }

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
    public BookOdd[] Odds { get; set; }

    [BsonElement("link")]
    public string OddsLink { get; set; }

    [BsonElement("inserted_at")]
    public DateTime InsertedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

public sealed record class BookOdd
{
    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("home")]
    public string HomeOdd { get; set; }

    [BsonElement("away")]
    public string AwayOdd { get; set; }
}