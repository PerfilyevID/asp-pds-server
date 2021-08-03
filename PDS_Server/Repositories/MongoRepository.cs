using CommonEnvironment.Elements;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS_Server.Repositories
{
    public class MongoRepository<T> : IMongoRepository<T>
        where T : DbElement
    {
        private readonly IMongoCollection<T> _collection;
        public MongoRepository(IMongoClient client)
        {
            var database = client.GetDatabase("RT-Cluster-Db");
            var collection = database.GetCollection<T>(typeof(T).Name.Remove(0,2));
            _collection = collection;
        }
        public async Task<ObjectId> Create(T element)
        {
            await _collection.InsertOneAsync(element);
            return element.Id;
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
        public async Task<IEnumerable<T>> Get()
        {
            IEnumerable<T> elements = await _collection.Find(_ => true).ToListAsync();
            return elements;
        }
        public async Task<bool> Update(ObjectId objectId, T element)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq(c => c.Id, objectId);
            if (typeof(T) == typeof(DbAccount))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbAccount).SecondName, (element as DbAccount).SecondName)
                    .Set(e => (e as DbAccount).ImageData, (element as DbAccount).ImageData)
                    .Set(e => (e as DbAccount).IsVerified, (element as DbAccount).IsVerified)
                    .Set(e => (e as DbAccount).Login, (element as DbAccount).Login)
                    .Set(e => (e as DbAccount).FirstName, (element as DbAccount).FirstName)
                    .Set(e => (e as DbAccount).Owner, (element as DbAccount).Owner)
                    .Set(e => (e as DbAccount).Password, (element as DbAccount).Password)
                    .Set(e => (e as DbAccount).Access, (element as DbAccount).Access)
                    .Set(e => (e as DbAccount).Role, (element as DbAccount).Role)
                    .Set(e => (e as DbAccount).Team, (element as DbAccount).Team);
                UpdateResult result = await _collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbPlugin))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbPlugin).Description, (element as DbPlugin).Description)
                    .Set(e => (e as DbPlugin).Name, (element as DbPlugin).Name)
                    .Set(e => (e as DbPlugin).Versions, (element as DbPlugin).Versions);
                UpdateResult result = await _collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbApplication))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbApplication).Changelog, (element as DbApplication).Changelog)
                    .Set(e => (e as DbApplication).Link, (element as DbApplication).Link)
                    .Set(e => (e as DbApplication).Version, (element as DbApplication).Version);
                UpdateResult result = await _collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            return false;
        }
    }
}
