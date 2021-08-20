using MongoDB.Bson;
using PDS_Server.Elements;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS_Server.Repositories
{
    public interface IMongoRepository<T> where T : DbElement
    {
        #region Default
        Task<ObjectId> Create(T instance);
        Task<T> Get(ObjectId objectId);
        Task<IEnumerable<T>> Get();
        Task<bool> Update(ObjectId objectId, T instance);
        Task<bool> Delete(ObjectId objectId);
        Task<bool> Delete(ObjectId[] objectIds);
        #endregion

        #region Teams
        Task<ObjectId> Create(T instance, string teamId);
        Task<T> Get(ObjectId objectId, string teamId);
        Task<IEnumerable<T>> Get(string teamId);
        Task<bool> Update(ObjectId objectId, T instance, string teamId);
        Task<bool> Delete(ObjectId objectId, string teamId);
        Task<bool> Delete(ObjectId[] objectIds, string teamId);
        #endregion
    }
}
