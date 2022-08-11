using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using WorkoutGlobal.VideoService.Api.Contracts;
using System.Linq;

namespace WorkoutGlobal.VideoService.Api.Repositories
{
    public class BaseRepository<TModel> : IBaseRepository<TModel>
        where TModel : IModel
    {
        private IConfiguration _configuration;
        private IMongoDatabase _database;
        private string _collectionName;


        public BaseRepository(IConfiguration configuration, string collectionName)
        {
            Configuration = configuration;

            var client = new MongoClient(Configuration.GetConnectionString("MongoDbVideoServiceConnectionString"));
            Database = client.GetDatabase(Configuration["MongoDbConnection:DatabaseName"]);

            CollectionName = collectionName;
        }

        public string CollectionName
        {
            get => _collectionName;
            set => _collectionName = string.IsNullOrWhiteSpace(value) 
                ? throw new ArgumentNullException(nameof(value), "Collection name cannot be null.")
                : value;
        }

        public IConfiguration Configuration
        {
            get => _configuration;
            private set => _configuration = value 
                ?? throw new ArgumentNullException(nameof(value), "Project configuration cannot be null.");
        }

        public IMongoDatabase Database
        {
            get => _database;
            set => _database = value 
                ?? throw new ArgumentNullException(nameof(value), "Database instance cannot be null");
        }

        public async Task<ObjectId> CreateAsync(TModel model)
        {
            await Database.GetCollection<TModel>(CollectionName).InsertOneAsync(model);

            return model.Id;
        }

        public async Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<TModel>.Filter.Eq("_id", id);

            await Database.GetCollection<TModel>(CollectionName).DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            var models = await Database.GetCollection<TModel>(CollectionName).FindAsync(_ => true);

            return await models.ToListAsync();
        }

        public async Task<TModel> GetModelAsync(ObjectId id)
        {
            var filter = Builders<TModel>.Filter.Eq("_id", id);

            var result = await Database.GetCollection<TModel>(CollectionName).FindAsync(filter);

            return result.FirstOrDefault();
        }

        public async Task UpdateAsync(TModel model)
        {
            var filter = Builders<TModel>.Filter.Eq("_id", model.Id);

            await Database.GetCollection<TModel>(CollectionName).ReplaceOneAsync(
                filter: filter,
                replacement: model);
        }
    }
}
