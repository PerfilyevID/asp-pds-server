using MongoDB.Bson;
using MongoDB.Driver;
using PDS_Server.Elements;
using PDS_Server.Elements.Communications;
using PDS_Server.Elements.Plugins;
using PDS_Server.Elements.Revit;
using System.Threading.Tasks;

namespace PDS_Server.Repositories
{
    public static class LocalMongoDbUtills<T>
        where T : DbElement
    {
        public static async Task<bool> Update(IMongoCollection<T> collection, ObjectId objectId, T element)
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
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbPlugin))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbPlugin).Description, (element as DbPlugin).Description)
                    .Set(e => (e as DbPlugin).Name, (element as DbPlugin).Name)
                    .Set(e => (e as DbPlugin).Versions, (element as DbPlugin).Versions);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbApplication))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbApplication).Changelog, (element as DbApplication).Changelog)
                    .Set(e => (e as DbApplication).Link, (element as DbApplication).Link)
                    .Set(e => (e as DbApplication).Version, (element as DbApplication).Version);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbTeam))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbTeam).MaxCount, (element as DbTeam).MaxCount)
                    .Set(e => (e as DbTeam).Name, (element as DbTeam).Name)
                    .Set(e => (e as DbTeam).Users, (element as DbTeam).Users)
                    .Set(e => (e as DbTeam).Owner, (element as DbTeam).Owner);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbReport))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbReport).Comment, (element as DbReport).Comment)
                    .Set(e => (e as DbReport).Description, (element as DbReport).Description)
                    .Set(e => (e as DbReport).IsClosed, (element as DbReport).IsClosed)
                    .Set(e => (e as DbReport).Issue, (element as DbReport).Issue)
                    .Set(e => (e as DbReport).Link, (element as DbReport).Link)
                    .Set(e => (e as DbReport).Time, (element as DbReport).Time);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbProject))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbProject).Color, (element as DbProject).Color)
                    .Set(e => (e as DbProject).Documents, (element as DbProject).Documents)
                    .Set(e => (e as DbProject).Name, (element as DbProject).Name)
                    .Set(e => (e as DbProject).Team, (element as DbProject).Team);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbDocument))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbDocument).Department, (element as DbDocument).Department)
                    .Set(e => (e as DbDocument).FoundByUser, (element as DbDocument).FoundByUser)
                    .Set(e => (e as DbDocument).FullPath, (element as DbDocument).FullPath)
                    .Set(e => (e as DbDocument).Name, (element as DbDocument).Name)
                    .Set(e => (e as DbDocument).IsCloudModel, (element as DbDocument).IsCloudModel)
                    .Set(e => (e as DbDocument).SyncCount, (element as DbDocument).SyncCount)
                    .Set(e => (e as DbDocument).Project, (element as DbDocument).Project);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbTask))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbTask).Chat, (element as DbTask).Chat)
                    .Set(e => (e as DbTask).CheckLists, (element as DbTask).CheckLists)
                    .Set(e => (e as DbTask).Description, (element as DbTask).Description)
                    .Set(e => (e as DbTask).From, (element as DbTask).From)
                    .Set(e => (e as DbTask).GltfData, (element as DbTask).GltfData)
                    .Set(e => (e as DbTask).ImageData, (element as DbTask).ImageData)
                    .Set(e => (e as DbTask).Name, (element as DbTask).Name)
                    .Set(e => (e as DbTask).To, (element as DbTask).To)
                    .Set(e => (e as DbTask).Watchers, (element as DbTask).Watchers)
                    .Set(e => (e as DbTask).ExpirationTime, (element as DbTask).ExpirationTime);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbChat))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbChat).LastChange, (element as DbChat).LastChange)
                    .Set(e => (e as DbChat).Messages, (element as DbChat).Messages);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbClashResult))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbClashResult).Clashes, (element as DbClashResult).Clashes)
                    .Set(e => (e as DbClashResult).CreationTime, (element as DbClashResult).CreationTime)
                    .Set(e => (e as DbClashResult).Description, (element as DbClashResult).Description)
                    .Set(e => (e as DbClashResult).ItemsCount, (element as DbClashResult).ItemsCount)
                    .Set(e => (e as DbClashResult).ItemsDone, (element as DbClashResult).ItemsDone)
                    .Set(e => (e as DbClashResult).LastChange, (element as DbClashResult).LastChange)
                    .Set(e => (e as DbClashResult).Name, (element as DbClashResult).Name)
                    .Set(e => (e as DbClashResult).Group, (element as DbClashResult).Group);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbDepartment))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbDepartment).Description, (element as DbDepartment).Description)
                    .Set(e => (e as DbDepartment).Name, (element as DbDepartment).Name);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbCheckResult))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbCheckResult).Data, (element as DbCheckResult).Data)
                    .Set(e => (e as DbCheckResult).Document, (element as DbCheckResult).Document)
                    .Set(e => (e as DbCheckResult).Project, (element as DbCheckResult).Project);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbFamily))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbFamily).Category, (element as DbFamily).Category)
                    .Set(e => (e as DbFamily).Description, (element as DbFamily).Description)
                    .Set(e => (e as DbFamily).Link, (element as DbFamily).Link)
                    .Set(e => (e as DbFamily).Name, (element as DbFamily).Name)
                    .Set(e => (e as DbFamily).Team, (element as DbFamily).Team)
                    .Set(e => (e as DbFamily).LOD, (element as DbFamily).LOD)
                    .Set(e => (e as DbFamily).ImageData, (element as DbFamily).ImageData)
                    .Set(e => (e as DbFamily).GltfData, (element as DbFamily).GltfData);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            else if (typeof(T) == typeof(DbFamilyCategory))
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(e => (e as DbFamilyCategory).Name, (element as DbFamilyCategory).Name)
                    .Set(e => (e as DbFamilyCategory).Children, (element as DbFamilyCategory).Children);
                UpdateResult result = await collection.UpdateOneAsync(filter, update);
                return result.ModifiedCount == 1;
            }
            return false;
        }
    }
}
