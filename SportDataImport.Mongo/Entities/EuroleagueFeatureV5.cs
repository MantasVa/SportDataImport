using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SportDataImport.Mongo.Attributes;

namespace SportDataImport.Mongo.Entities;

[BsonCollectionName(Constants.EuroleagueFeaturesV5CollectionName)]
public record EuroleagueFeatureV5
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("seasonCode")]
    public string SeasonCode { get; init; }

    [BsonElement("round")]
    public long Round { get; init; }

    [BsonElement("home")]
    public string Home { get; init; }

    [BsonElement("bookName")]
    public string BookName { get; init; }

    [BsonElement("homeLast1")]
    public int? HomeLast1 { get; init; }

    [BsonElement("homeLast2")]
    public int? HomeLast2 { get; init; }

    [BsonElement("homeLast3")]
    public int? HomeLast3 { get; init; }

    [BsonElement("homeLast4")]
    public int? HomeLast4 { get; init; }

    [BsonElement("homeLast5")]
    public int? HomeLast5 { get; init; }

    [BsonElement("homeIsFinalFour1")]
    public int HomeIsFinalFour1 { get; init; }

    [BsonElement("homeIsFinalFour2")]
    public int HomeIsFinalFour2 { get; init; }

    [BsonElement("homeIsFinalFour3")]
    public int HomeIsFinalFour3 { get; init; }

    [BsonElement("homeWinPercentage")]
    public float HomeWinPercentage { get; init; }

    [BsonElement("homeOdds")]
    public float HomeOdds { get; init; }

    [BsonElement("away")]
    public string Away { get; init; }

    [BsonElement("awayLast1")]
    public int? AwayLast1 { get; init; }

    [BsonElement("awayLast2")]
    public int? AwayLast2 { get; init; }

    [BsonElement("awayLast3")]
    public int? AwayLast3 { get; init; }

    [BsonElement("awayLast4")]
    public int? AwayLast4 { get; init; }

    [BsonElement("awayLast5")]
    public int? AwayLast5 { get; init; }

    [BsonElement("awayIsFinalFour1")]
    public int AwayIsFinalFour1 { get; init; }

    [BsonElement("awayIsFinalFour2")]
    public int AwayIsFinalFour2 { get; init; }

    [BsonElement("awayIsFinalFour3")]
    public int AwayIsFinalFour3 { get; init; }

    [BsonElement("awayWinPercentage")]
    public float AwayWinPercentage { get; init; }

    [BsonElement("awayOdds")]
    public float AwayOdds { get; init; }

    [BsonElement("homeWin")]
    public bool HomeWin { get; init; }

    public EuroleagueFeatureV5()
    {
        Id = ObjectId.GenerateNewId();
    }
}
