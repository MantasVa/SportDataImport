using MongoDB.Driver;
using System.Linq.Expressions;

namespace SportDataImport.Mongo.Interfaces;

public interface IMongoService<T> where T : class
{
    Task<long> Count();

    Task<List<T>> GetBy(Expression<Func<T, bool>> filter);

    Task<List<T>> GetAll();

    Task Insert(T entity);

    Task InsertMany(List<T> entities);

    Task<DeleteResult> DeleteBy(Expression<Func<T, bool>> filter);
}
