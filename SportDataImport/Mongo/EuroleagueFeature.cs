using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SportDataImport.Mongo;

internal record EuroleagueFeature
{
    [BsonId]
    public ObjectId Id { get; set; }
    [BsonElement("seasonCode")]
    public string SeasonCode { get; init; }
    [BsonElement("round")]
    public long Round { get; init; }
    [BsonElement("home")]
    public string Home { get; init; }
    [BsonElement("homeLast1")]
    public int? HomeLast1 { get; init; }
    [BsonElement("homeLast2")]
    public int? HomeLast2 { get; init; }
    [BsonElement("homeLast3")]
    public int? HomeLast3 { get; init; }
    [BsonElement("homeLast4")]
    public int? HomeLast4 { get; init; }

    [BsonElement("homeIsFinalFour1")]
    public int HomeIsFinalFour1 { get; init; }

    [BsonElement("homeIsFinalFour2")]
    public int HomeIsFinalFour2 { get; init; }

    [BsonElement("homeIsFinalFour3")]
    public int HomeIsFinalFour3 { get; init; }

    [BsonElement("homeWinPercentage")]
    public float HomeWinPercentage { get; init; }
    [BsonElement("road")]
    public string Road { get; init; }
    [BsonElement("roadLast1")]
    public int? RoadLast1 { get; init; }
    [BsonElement("roadLast2")]
    public int? RoadLast2 { get; init; }
    [BsonElement("roadLast3")]
    public int? RoadLast3 { get; init; }
    [BsonElement("roadLast4")]
    public int? RoadLast4 { get; init; }

    [BsonElement("roadIsFinalFour1")]
    public int RoadIsFinalFour1 { get; init; }

    [BsonElement("roadIsFinalFour2")]
    public int RoadIsFinalFour2 { get; init; }

    [BsonElement("roadIsFinalFour3")]
    public int RoadIsFinalFour3 { get; init; }

    [BsonElement("roadWinPercentage")]
    public float RoadWinPercentage { get; init; }

    [BsonElement("homeWin")]
    public bool HomeWin { get; init; }

    public EuroleagueFeature()
    {
        Id = ObjectId.GenerateNewId();
    }
}