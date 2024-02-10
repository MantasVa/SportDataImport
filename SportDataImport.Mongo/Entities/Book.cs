using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SportDataImport.Mongo.Attributes;
using SportDataImport.Domain.Enums;

namespace SportDataImport.Mongo.Entities;

[BsonCollectionName(Constants.BookCollectionName)]
public sealed record class Book
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("competition")]
    public Competition Competition { get; set; }

    [BsonElement("books")]
    public string[] Books { get; set; } = null!;
}

