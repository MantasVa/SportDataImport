using MongoDB.Bson.Serialization.Attributes;
using SportDataImport.Domain.Enums;
using SportDataImport.Mongo.Attributes;

namespace SportDataImport.Mongo.Entities;

[BsonCollectionName(Constants.EuroleagueFeaturesV4CollectionName)]
public record EuroleagueFeatureV4 : EuroleagueFeatureV2
{
    [BsonElement("weekType")]
    public float WeekType { get; set; }

    public EuroleagueFeatureV4(WeekType weekType, EuroleagueFeatureV2 original) 
        : base(original.HomeWinAgainstRoadPercentage, original.RoadWinAgainstHomePercentage, original)
    {
        WeekType = weekType.ToFloat();
    }
}
