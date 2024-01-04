namespace SportDataImport;

internal static class Constants
{
    public static string ConnectionString = "mongodb://localhost:27017";
    public static string DatabaseName = "sports";

    public static string TeamCollectionName = "teams";
    public static string ScheduleCollectionName = "schedule";
    public static string GameCollectionName = "basketball_games";
    public static string EuroleagueFeaturesCollectionName = "euroleague_features";
    public static string EuroleagueFeaturesV2CollectionName = "euroleague_features_v2";
    public static string EuroleagueFeaturesV3CollectionName = "euroleague_features_v3";
    public static string EuroleagueFeaturesV4CollectionName = "euroleague_features_v4";

    public static string[] ModernEuroleagueSeasons = new[] { "E2000", "E2001", "E2002", "E2003", "E2004", "E2005", "E2006", "E2007", "E2008", "E2009", "E2010",
        "E2011", "E2012", "E2013", "E2014", "E2015", "E2016", "E2017", "E2018", "E2019", "E2020", "E2021", "E2022", "E2023", };
}
