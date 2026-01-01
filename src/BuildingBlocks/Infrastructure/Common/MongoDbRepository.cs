using Contracts.Domains.Interfaces;
using Contracts.Domains;
using Infrastructure.Extensions;
using MongoDB.Driver;
using Shared.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
    public class MongoDbRepository<T> : IMongoDbRepositoryBase<T> where T : MongoEntity
    {
        public IMongoDatabase Database { get; }

        public MongoDbRepository(IMongoClient client, MongoDbSettings settings)
        {
            Database = client.GetDatabase(settings.DatabaseName);
        }

        protected virtual IMongoCollection<T> Collection => Database.GetCollection<T>(GetCollectionName());

        public Task CreateAsync(T entity)
        {
            return Collection.InsertOneAsync(entity);
        }

        public Task DeleteAsync(string id)
        {
            return Collection.DeleteOneAsync(x => x.Id.Equals(id));
        }

        public IMongoCollection<T> FindAll(ReadPreference? readPreference = null)
        {
            return Database.WithReadPreference(readPreference ?? ReadPreference.Primary)
                .GetCollection<T>(GetCollectionName());
        }

        public Task UpdateAsync(T entity)
        {
            Expression<Func<T, string>> func = f => f.Id;
            var value = (string)entity.GetType().GetProperty(func.Body.ToString().Split(".")[1])?.GetValue(entity, null);
            var filder = Builders<T>.Filter.Eq(func, value);
            return Collection.ReplaceOneAsync(filder, entity);
        }

        private static string GetCollectionName()
        {
            return (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault() as BsonCollectionAttribute)?.CollectionName;
        }
    }
}
