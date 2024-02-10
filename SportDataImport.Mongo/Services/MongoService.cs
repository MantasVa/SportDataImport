using MongoDB.Driver;
using SportDataImport.Mongo.Attributes;
using SportDataImport.Mongo.Interfaces;
using System.Linq.Expressions;

namespace SportDataImport.Mongo.Services;

public class MongoService<T> : IMongoService<T> where T : class
{
    private readonly IMongoCollection<T> _collection;

    public MongoService()
    {
        var database = new MongoClient(Constants.ConnectionString).GetDatabase(Constants.DatabaseName);
        var collectionName = GetCollectionName();
        _collection = database.GetCollection<T>(collectionName);
    }

    public async Task<long> Count()
    {
        return await _collection.CountDocumentsAsync(FilterDefinition<T>.Empty);
    }

    public async Task<T> GetOne(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).FirstAsync();
    }

    public async Task<List<T>> GetBy(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<List<T>> GetAll()
    {
        return await _collection.Find(FilterDefinition<T>.Empty).ToListAsync();
    }

    public async Task Insert(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task InsertMany(List<T> entities)
    {
        await _collection.InsertManyAsync(entities);
    }

    public async Task<DeleteResult> DeleteBy(Expression<Func<T, bool>> filter)
    {
        return await _collection.DeleteManyAsync(filter);
    }

    private static string GetCollectionName()
    {
        return (typeof(T).GetCustomAttributes(typeof(BsonCollectionNameAttribute), true).First() as BsonCollectionNameAttribute)!.Name;
    }
}
