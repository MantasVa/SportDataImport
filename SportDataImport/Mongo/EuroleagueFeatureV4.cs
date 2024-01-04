using MongoDB.Bson.Serialization.Attributes;
using SportDataImport.Enums;

namespace SportDataImport.Mongo;

internal record EuroleagueFeatureV4 : EuroleagueFeatureV2
{
    [BsonElement("weekType")]
    public float WeekType { get; set; }

    public EuroleagueFeatureV4(WeekType weekType, EuroleagueFeatureV2 original) 
        : base(original.HomeWinAgainstRoadPercentage, original.RoadWinAgainstHomePercentage, original)
    {
        WeekType = weekType.ToFloat();
    }
}
