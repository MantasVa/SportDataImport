namespace SportDataImport.Mongo;

internal class Constants
{
    public const string ConnectionString = "mongodb://localhost:27017";
    public const string DatabaseName = "sports";

    public const string TeamCollectionName = "teams";
    public const string ScheduleCollectionName = "schedule";
    public const string GameCollectionName = "basketball_games";
    public const string GameOddsCollectionName = "odds";
    public const string BookCollectionName = "books";
    public const string EuroleagueFeaturesCollectionName = "euroleague_features";
    public const string EuroleagueFeaturesV2CollectionName = "euroleague_features_v2";
    public const string EuroleagueFeaturesV3CollectionName = "euroleague_features_v3";
    public const string EuroleagueFeaturesV4CollectionName = "euroleague_features_v4";
    public const string EuroleagueFeaturesV5CollectionName = "euroleague_features_v5";
}
