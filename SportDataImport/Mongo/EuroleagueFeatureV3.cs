using MongoDB.Bson.Serialization.Attributes;

namespace SportDataImport.Mongo;

internal sealed record EuroleagueFeatureV3 : EuroleagueFeatureV2
{
    [BsonElement("homeLocalWinsPercentage")]
    public float HomeLocalWinsPercentage { get; }

    [BsonElement("roadLocalWinsPercentage")]
    public float RoadAwayWinsPercentage { get; }

    public EuroleagueFeatureV3(
        float homeLocalWinsPercentage,
        float roadAwayWinsPercentage,
        EuroleagueFeatureV2 featureV2) : 
        base(
            featureV2.HomeWinAgainstRoadPercentage, 
            featureV2.RoadWinAgainstHomePercentage,
            new EuroleagueFeature
            {
                SeasonCode = featureV2.SeasonCode,
                Round = featureV2.Round,
                Home = featureV2.Home,
                HomeLast1 = featureV2.HomeLast1,
                HomeLast2 = featureV2.HomeLast2,
                HomeLast3 = featureV2.HomeLast3,
                HomeLast4 = featureV2.HomeLast4,
                HomeIsFinalFour1 = featureV2.HomeIsFinalFour1,
                HomeIsFinalFour2 = featureV2.HomeIsFinalFour2,
                HomeIsFinalFour3 = featureV2.HomeIsFinalFour3,
                HomeWinPercentage = featureV2.HomeWinPercentage,
                Road = featureV2.Road,
                RoadLast1 = featureV2.RoadLast1,
                RoadLast2 = featureV2.RoadLast2,
                RoadLast3 = featureV2.RoadLast3,
                RoadLast4 = featureV2.RoadLast4,
                RoadIsFinalFour1 = featureV2.RoadIsFinalFour1,
                RoadIsFinalFour2 = featureV2.RoadIsFinalFour2,
                RoadIsFinalFour3 = featureV2.RoadIsFinalFour3,
                RoadWinPercentage = featureV2.RoadWinPercentage,
                HomeWin = featureV2.HomeWin,
            })
    {
        HomeLocalWinsPercentage = homeLocalWinsPercentage;
        RoadAwayWinsPercentage = roadAwayWinsPercentage;
    }
}
