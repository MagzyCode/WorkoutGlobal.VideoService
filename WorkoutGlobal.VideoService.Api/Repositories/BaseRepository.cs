using MongoDB.Bson;
using MongoDB.Driver;
using WorkoutGlobal.VideoService.Api.Contracts;
using MongoDB.Driver.GridFS;

namespace WorkoutGlobal.VideoService.Api.Repositories
{
    /// <summary>
    /// Base repository for working with Mongo database.
    /// </summary>
    /// <typeparam name="TModel">Model type.</typeparam>
    public class BaseRepository<TModel> : IBaseRepository<TModel>
        where TModel : IModel
    {
        private IConfiguration _configuration;
        private IMongoDatabase _database;
        private string _collectionName;
        private IGridFSBucket _gridFSBucket;

        /// <summary>
        /// Ctor for base repository.
        /// </summary>
        /// <param name="configuration">Project configuration.</param>
        /// <param name="collectionName">Default collection name.</param>
        public BaseRepository(IConfiguration configuration, string collectionName)
        {
            Configuration = configuration;

            var client = new MongoClient(Configuration["ConnectionStrings:MongoDbVideoServiceConnectionString"]);
            Database = client.GetDatabase(Configuration["MongoDbConnection:DatabaseName"]);

            CollectionName = collectionName;

            GridFSBucket = new GridFSBucket(Database);
        }

        /// <summary>
        /// GridFS instance.
        /// </summary>
        public IGridFSBucket GridFSBucket
        {
            get => _gridFSBucket;
            set => _gridFSBucket = value
                ?? throw new ArgumentNullException(nameof(value), "GridFS instance cannot be null.");
        }

        /// <summary>
        /// Collection name.
        /// </summary>
        public string CollectionName
        {
            get => _collectionName;
            set => _collectionName = string.IsNullOrWhiteSpace(value) 
                ? throw new ArgumentNullException(nameof(value), "Collection name cannot be null.")
                : value;
        }

        /// <summary>
        /// Project configuration.
        /// </summary>
        public IConfiguration Configuration
        {
            get => _configuration;
            private set => _configuration = value 
                ?? throw new ArgumentNullException(nameof(value), "Project configuration cannot be null.");
        }

        /// <summary>
        /// Current used database.
        /// </summary>
        public IMongoDatabase Database
        {
            get => _database;
            set => _database = value 
                ?? throw new ArgumentNullException(nameof(value), "Database instance cannot be null");
        }

        /// <summary>
        /// Gerenal creation of new model.
        /// </summary>
        /// <param name="model">Creation model.</param>
        /// <returns>A task that represents asynchronous Create operation.</returns>
        public async Task<ObjectId> CreateAsync(TModel model)
        {
            await Database.GetCollection<TModel>(CollectionName).InsertOneAsync(model);

            return model.Id;
        }

        /// <summary>
        /// Gerenal deletion of existed model.
        /// </summary>
        /// <param name="id">Deleting model.</param>
        public async Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<TModel>.Filter.Eq("_id", id);

            await Database.GetCollection<TModel>(CollectionName).DeleteOneAsync(filter);
        }

        /// <summary>
        /// General getting of all models.
        /// </summary>
        /// <returns>IQueryable list of models.</returns>
        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            var models = await Database.GetCollection<TModel>(CollectionName).FindAsync(_ => true);

            return await models.ToListAsync();
        }

        /// <summary>
        /// Gerenal getting of single model by id.
        /// </summary>
        /// <param name="id">Model id.</param>
        /// <returns>Existed model.</returns>
        public async Task<TModel> GetModelAsync(ObjectId id)
        {
            var filter = Builders<TModel>.Filter.Eq("_id", id);

            var result = await Database.GetCollection<TModel>(CollectionName).FindAsync(filter);

            return result.FirstOrDefault();
        }

        /// <summary>
        /// General update action for existed model.
        /// </summary>
        /// <param name="model">Updated model.</param>
        public async Task UpdateAsync(TModel model)
        {
            var filter = Builders<TModel>.Filter.Eq("_id", model.Id);

            await Database.GetCollection<TModel>(CollectionName).ReplaceOneAsync(
                filter: filter,
                replacement: model);
        }

        /// <summary>
        /// Add file in GridFS.
        /// </summary>
        /// <param name="videoName">Video file name.</param>
        /// <param name="videoFile">Video file data.</param>
        /// <returns>Returns generated id for file.</returns>
        public async Task<ObjectId> AddFileAsync(string videoName, byte[] videoFile)
        {
            var createdId = await GridFSBucket.UploadFromBytesAsync(videoName, videoFile);

            return createdId;
        }
    }
}
