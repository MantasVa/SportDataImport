using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using SportDataImport.Enums;

namespace SportDataImport.Mongo;

internal sealed record ScheduledEvent : IEquatable<ScheduledEvent>
{
    public ScheduledEvent(
        long gameCode, 
        long round, 
        string seasonCode, 
        long year,
        DateTimeOffset localDate,
        DateTimeOffset utcDate,
        Competition competition, 
        Phase phaseType, 
        string home, 
        string road)
    {
        Id = ObjectId.GenerateNewId();
        GameCode = gameCode;
        Round = round;
        SeasonCode = seasonCode;
        Year = year;
        LocalDate = localDate;
        UtcDate = utcDate;
        Competition = competition;
        PhaseType = phaseType;
        Home = home;
        Road = road;
    }

    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("gameCode")]
    public long GameCode { get; set; }

    [BsonElement("round")]
    public long Round { get; set; }

    [BsonElement("seasonCode")]
    public string SeasonCode { get; set; }

    [BsonElement("year")]
    public long Year { get; set; }

    [BsonElement("localDate")]
    [BsonSerializer(typeof(DateTimeOffsetSerializer))]
    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset LocalDate { get; set; }

    [BsonElement("utcDate")]
    [BsonSerializer(typeof(DateTimeOffsetSerializer))]
    [BsonRepresentation(BsonType.Document)]
    public DateTimeOffset UtcDate { get; set; }

    [BsonElement("competition")]
    public Competition Competition { get; set; }

    [BsonElement("phase")]
    public Phase PhaseType { get; set; }

    [BsonElement("home")]
    public string Home { get; set; }

    [BsonElement("road")]
    public string Road { get; set; }


    public bool Equals(ScheduledEvent? scheduledEvent)
    {
        return
            GameCode == scheduledEvent?.GameCode &&
            Round == scheduledEvent?.Round &&
            SeasonCode == scheduledEvent?.SeasonCode &&
            Year == scheduledEvent?.Year &&
            UtcDate == scheduledEvent?.UtcDate &&
            Competition == scheduledEvent?.Competition &&
            PhaseType == scheduledEvent?.PhaseType &&
            Home == scheduledEvent?.Home &&
            Road == scheduledEvent?.Road;
    }
}
