using CommonEnvironment.Elements;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS_Server.Repositories
{
    public interface IMongoRepository<T> where T : DbElement
    {
        Task<ObjectId> Create(T instance);
        Task<T> Get(ObjectId objectId);
        Task<IEnumerable<T>> Get();
        Task<bool> Update(ObjectId objectId, T instance);
        Task<bool> Delete(ObjectId objectId);
    }
}
