using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SportDataImport.Clients;
using SportDataImport.Domain;
using SportDataImport.Mongo.Entities;
using SportDataImport.Mongo.Interfaces;

namespace SportDataImport.Jobs;

internal interface IScheduleImportJob
{
    Task ImportEuroleagueScheduleAsync();
}

internal class ScheduleImportJob : IScheduleImportJob
{
    private readonly IMongoService<Game> _gamesCollection;
    private readonly IMongoService<ScheduledEvent> _scheduleCollection;
    private readonly ILogger<ScheduleImportJob> _logger;
    private readonly IEuroleagueClient _euroleagueClient;

    public ScheduleImportJob(
        ILogger<ScheduleImportJob> logger,
        IEuroleagueClient euroleagueClient,
        IMongoService<ScheduledEvent> scheduleCollection,
        IMongoService<Game> gamesCollection)
    {
        _logger = logger;
        _euroleagueClient = euroleagueClient;
        _scheduleCollection = scheduleCollection;
        _gamesCollection = gamesCollection;
    }

    public async Task ImportEuroleagueScheduleAsync()
    {
        var currentEuroleagueSeasonCode = EuroleagueHelper.GetCurrentEuroleagueSeasonCode();
        var (success, events) = await _euroleagueClient.GetGames(currentEuroleagueSeasonCode);

        var schedule = await _scheduleCollection.GetAll();
        var scheduleAdded = 0;

        if (!success)
        {
            _logger.LogWarning("Could not retrieve schedule from euroleague client");
            return;
        }

        foreach (var game in events)
        {
            // Game is already played. Check if we have a record in games collection, put into schedule if not.
            if (game.Played.HasValue && game.Played.Value)
            {
                var storedGames = await _gamesCollection.GetBy(x => x.SeasonCode == game.Season!.Code && x.GameCode == game.GameCode);

                if (storedGames.Any())
                {
                    if (storedGames.Count > 1)
                    {
                        _logger.LogError("We have several stored games in season {SeasonCode} and game code {GameCode}", game.Season!.Code, game.GameCode);
                    }

                    continue;
                }
            }

            var scheduledEvent = game.ToScheduledEvent();
            var storedEvent = schedule.FirstOrDefault(x => x.SeasonCode == scheduledEvent.SeasonCode && x.GameCode == scheduledEvent.GameCode);

            if (storedEvent == null)
            {
                scheduleAdded++;
                _logger.LogInformation("Storing new scheduled event in season {SeasonCode} and game code {GameCode}", scheduledEvent.SeasonCode, scheduledEvent.GameCode);
                await _scheduleCollection.Insert(scheduledEvent);
            }
            else if (!storedEvent.Equals(scheduledEvent))
            {
                _logger.LogInformation("Stored event is different from the client one for {GameCode}", scheduledEvent.GameCode);
                await _scheduleCollection.DeleteBy(x => x.SeasonCode == scheduledEvent.SeasonCode && x.GameCode == scheduledEvent.GameCode);
                await _scheduleCollection.Insert(scheduledEvent);
            }
        }

        _logger.LogInformation("{Job} is finished with {ScheduleAdded} schedules added.", nameof(ScheduleImportJob), scheduleAdded);
    }
}
