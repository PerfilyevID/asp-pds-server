using CommonEnvironment.Elements;
using CommonEnvironment.Elements.Revit;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS_Server.Repositories
{
    public class MongoRepository<T> : IMongoRepository<T>
        where T : DbElement
    {
        private readonly Dictionary<string, IMongoCollection<T>> _targetCollections = new Dictionary<string, IMongoCollection<T>>();
        private readonly IMongoCollection<T> _collection;
        private readonly IMongoDatabase _dataBase;

        private void PrepareTeamCollection(string teamId)
        {
            if (!_targetCollections.Keys.ToArray().Contains(teamId))
            {
                _targetCollections.Add(teamId, _dataBase.GetCollection<T>(string.Format("{0}_{1}", typeof(T).Name.Remove(0, 2), teamId)));
            }
        }
        public MongoRepository(IMongoClient client)
        {
            _dataBase = client.GetDatabase("rt-db");
            var collection = _dataBase.GetCollection<T>(typeof(T).Name.Remove(0,2));
            _collection = collection;
        }
        public async Task<ObjectId> Create(T element)
        {
            await _collection.InsertOneAsync(element);
            return element.Id;
        }
        public async Task<ObjectId> Create(T element, string teamId)
        {
            PrepareTeamCollection(teamId);
            if (_targetCollections.TryGetValue(teamId, out IMongoCollection<T> teamCollection))
            {
                await teamCollection.InsertOneAsync(element);
            }
            return element.Id;
        }
        public async Task<bool> Delete(ObjectId objectId, string teamId)
        {
            PrepareTeamCollection(teamId);
            if (_targetCollections.TryGetValue(teamId, out IMongoCollection<T> teamCollection))
            {
                FilterDefinition<T> filter = Builders<T>.Filter.Eq(c => c.Id, objectId);
                DeleteResult result = await teamCollection.DeleteOneAsync(filter);
                return result.DeletedCount == 1;
            }
            return false;
        }
        public async Task<bool> Delete(ObjectId objectId)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq(c => c.Id, objectId);
            DeleteResult result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount == 1;
        }
        public async Task<T> Get(ObjectId objectId)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq(c => c.Id, objectId);
            T element = await _collection.Find(filter).FirstOrDefaultAsync();
            return element;
        }
        public async Task<T> Get(ObjectId objectId, string teamId)
        {
            PrepareTeamCollection(teamId);
            if (_targetCollections.TryGetValue(teamId, out IMongoCollection<T> teamCollection))
            {
                FilterDefinition<T> filter = Builders<T>.Filter.Eq(c => c.Id, objectId);
                T element = await teamCollection.Find(filter).FirstOrDefaultAsync();
                return element;
            }
            return null;
        }
        public async Task<IEnumerable<T>> Get()
        {
            IEnumerable<T> elements = await _collection.Find(_ => true).ToListAsync();
            return elements;
        }
        public async Task<IEnumerable<T>> Get(string teamId)
        {
            PrepareTeamCollection(teamId);
            if (_targetCollections.TryGetValue(teamId, out IMongoCollection<T> teamCollection))
            {
                IEnumerable<T> elements = await _collection.Find(_ => true).ToListAsync();
                return elements;
            }
            return new T[] { };
        }
        public async Task<bool> Update(ObjectId objectId, T element)
        {
            return await LocalMongoDbUtills<T>.Update(_collection, objectId, element);
        }
        public async Task<bool> Update(ObjectId objectId, T element, string teamId)
        {
            PrepareTeamCollection(teamId);
            if (_targetCollections.TryGetValue(teamId, out IMongoCollection<T> teamCollection))
            {
                return await LocalMongoDbUtills<T>.Update(teamCollection, objectId, element);
            }
            return false;
        }
    }
}
