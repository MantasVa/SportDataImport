using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using SportDataImport.Clients;
using SportDataImport.Mongo;

namespace SportDataImport.Jobs;

internal interface IScheduleOutcomeEvaluatorJob
{
    Task ImportEuroleagueScheduleAsync();
}

internal class ScheduleOutcomeEvaluatorJob : IScheduleOutcomeEvaluatorJob
{
    private readonly ILogger<ScheduleOutcomeEvaluatorJob> _logger;
    private readonly IEuroleagueClient _euroleagueClient;

    public ScheduleOutcomeEvaluatorJob(
        ILogger<ScheduleOutcomeEvaluatorJob> logger,
        IEuroleagueClient euroleagueClient)
    {
        _logger = logger;
        _euroleagueClient = euroleagueClient;
    }

    public async Task ImportEuroleagueScheduleAsync()
    {
        var database = new MongoClient(Constants.ConnectionString).GetDatabase(Constants.DatabaseName);
        var scheduleCollection = database.GetCollection<ScheduledEvent>(Constants.ScheduleCollectionName);
        var playSchedule = await scheduleCollection.Find(x => x.UtcDate <= DateTime.UtcNow).ToListAsync();

        var gamesCollection = database.GetCollection<Game>(Constants.GameCollectionName);
        var gamesAdded = 0;

        foreach (var @event in playSchedule.OrderBy(x => x.UtcDate))
        {
            var (isSuccess, game) = await _euroleagueClient.GetGame(@event.SeasonCode, @event.GameCode);

            if (!isSuccess)
            {
                _logger.LogWarning("Game call is not successful in season {SeasonCode} game code {GameCode}", @event.SeasonCode, @event.GameCode);
                continue;
            }

            if (game == null)
            {
                throw new ArgumentOutOfRangeException(nameof(game));
            }

            if (game.Played.HasValue && !game!.Played!.Value)
            {
                _logger.LogInformation("Season {SeasonCode} game with code {GameCode} and start date {StartDate} (UTC) is not played yet",
                    @event.SeasonCode, @event.GameCode, @event.UtcDate.UtcDateTime);
                continue;
            }

            var storedGames = await (await gamesCollection.FindAsync(x => x.SeasonCode == @event.SeasonCode && x.GameCode == @event.GameCode)).ToListAsync();
            if (storedGames.Any())
            {
                _logger.LogWarning("Scheduled event with season code {SeasonCode} and code {GameCode} is already present in collection", @event.SeasonCode, @event.GameCode);
                continue;
            }

            var result = await scheduleCollection.DeleteManyAsync(x => x.SeasonCode == @event.SeasonCode && x.GameCode == @event.GameCode);
            if (result.DeletedCount == 0 || !result.IsAcknowledged)
            {
                _logger.LogError("Schedule deletion failed for season {SeasonCode} game {GameCode} with result {DeleteCount}, {IsAcknowledged}",
                    @event.SeasonCode, @event.GameCode, result.DeletedCount, result.IsAcknowledged);
            }

            if (result.DeletedCount > 1)
            {
                _logger.LogError("Schedule deletion deleted more than one scheduled event for season {SeasonCode} game {GameCode} with result {DeleteCount}",
                    @event.SeasonCode, @event.GameCode, result.DeletedCount);
            }

            var gameEntity = game.ToGame().ToBsonDocument();
            var collection = database.GetCollection<BsonDocument>(Constants.GameCollectionName);
            await collection.InsertOneAsync(gameEntity);
            gamesAdded++;

            _logger.LogInformation("Game added in Season {SeasonCode} game code {GameCode}", @event.SeasonCode, @event.GameCode);
        }

        _logger.LogInformation("{Job} is finished with {GamesAdded} games added.", nameof(ScheduleOutcomeEvaluatorJob), gamesAdded);
    }
}
