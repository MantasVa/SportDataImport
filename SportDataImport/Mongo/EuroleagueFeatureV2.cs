using MongoDB.Bson.Serialization.Attributes;

namespace SportDataImport.Mongo;

internal record EuroleagueFeatureV2 : EuroleagueFeature
{

    [BsonElement("homeWinAgainstRoadPercentage")]
    public float HomeWinAgainstRoadPercentage { get; init; }

    [BsonElement("roadWinAgainstHomePercentage")]
    public float RoadWinAgainstHomePercentage { get; init; }

    public EuroleagueFeatureV2(float localTeamAgainstRoadTeamWinPercentage, float roadTeamAgainstLocalTeamWinPercentage, EuroleagueFeature feature)
    {
        HomeWinAgainstRoadPercentage = localTeamAgainstRoadTeamWinPercentage;
        RoadWinAgainstHomePercentage = roadTeamAgainstLocalTeamWinPercentage;

        SeasonCode = feature.SeasonCode;
        Round = feature.Round;
        Home = feature.Home;
        HomeLast1 = feature.HomeLast1;
        HomeLast2 = feature.HomeLast2;
        HomeLast3 = feature.HomeLast3;
        HomeLast4 = feature.HomeLast4;
        HomeIsFinalFour1 = feature.HomeIsFinalFour1;
        HomeIsFinalFour2 = feature.HomeIsFinalFour2;
        HomeIsFinalFour3 = feature.HomeIsFinalFour3;
        HomeWinPercentage = feature.HomeWinPercentage;
        Road = feature.Road;
        RoadLast1 = feature.RoadLast1;
        RoadLast2 = feature.RoadLast2;
        RoadLast3 = feature.RoadLast3;
        RoadLast4 = feature.RoadLast4;
        RoadIsFinalFour1 = feature.RoadIsFinalFour1;
        RoadIsFinalFour2 = feature.RoadIsFinalFour2;
        RoadIsFinalFour3 = feature.RoadIsFinalFour3;
        RoadWinPercentage = feature.RoadWinPercentage;
        HomeWin = feature.HomeWin;
    }
}
