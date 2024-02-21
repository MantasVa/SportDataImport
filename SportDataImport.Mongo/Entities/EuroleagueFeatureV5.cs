using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SportDataImport.Mongo.Attributes;

namespace SportDataImport.Mongo.Entities;

[BsonCollectionName(Constants.EuroleagueFeaturesV5CollectionName)]
public record EuroleagueFeatureV5
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("season")]
    public string Season { get; init; }

    [BsonElement("round")]
    public long Round { get; init; }

    [BsonElement("home")]
    public string Home { get; init; }

    [BsonElement("bookName1")]
    public string BookName1 { get; init; }

    [BsonElement("bookName2")]
    public string BookName2 { get; init; }

    [BsonElement("bookName3")]
    public string BookName3 { get; init; }

    [BsonElement("insertedAt")]
    public DateTime InsertedAt { get; init; }

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

    [BsonElement("homeOdds1")]
    public double HomeOdds1 { get; init; }

    [BsonElement("homeOdds2")]
    public double HomeOdds2 { get; init; }

    [BsonElement("homeOdds3")]
    public double HomeOdds3 { get; init; }

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

    [BsonElement("awayOdds1")]
    public double AwayOdds1 { get; init; }

    [BsonElement("awayOdds2")]
    public double AwayOdds2 { get; init; }

    [BsonElement("awayOdds3")]
    public double AwayOdds3 { get; init; }

    [BsonElement("homeWin")]
    public bool HomeWin { get; init; }

    public EuroleagueFeatureV5()
    {
        Id = ObjectId.GenerateNewId();
    }
}
