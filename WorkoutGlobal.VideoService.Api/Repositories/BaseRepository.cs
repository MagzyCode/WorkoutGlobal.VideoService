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
        public BaseRepository(IConfiguration configuration)
        {
            Configuration = configuration;

            var client = new MongoClient(Configuration["ConnectionStrings:MongoDbVideoServiceConnectionString"]);

            Database = client.GetDatabase(Configuration["MongoDbConnection:DatabaseName"]); 
            GridFSBucket = new GridFSBucket(Database);
        }

        /// <summary>
        /// Ctor for base repository.
        /// </summary>
        /// <param name="configuration">Project configuration.</param>
        /// <param name="collectionName">Default collection name.</param>
        public BaseRepository(IConfiguration configuration, string collectionName)
            : this(configuration)
        {
            CollectionName = collectionName;
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
        /// Creation of new model in collection.
        /// </summary>
        /// <param name="model">Creation model.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <returns>Returns generated id for created model.</returns>
        public async Task<ObjectId> CreateAsync(TModel model, string collectionName)
        {
            await Database.GetCollection<TModel>(collectionName).InsertOneAsync(model);

            return model.Id;
        }

        /// <summary>
        /// Creation of new model in collection. Selected collection base on property CollectionName.
        /// </summary>
        /// <param name="model">Creation model.</param>
        /// <returns>A task that represents asynchronous Create operation.</returns>
        public async Task<ObjectId> CreateAsync(TModel model)
        {
            return await CreateAsync(model, CollectionName);
        }

        /// <summary>
        /// Gerenal deletion of existed model.
        /// </summary>
        /// <param name="id">Deleting model id.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <returns></returns>
        public async Task DeleteAsync(ObjectId id, string collectionName)
        {
            var filter = Builders<TModel>.Filter.Eq("_id", id);

            await Database.GetCollection<TModel>(collectionName).DeleteOneAsync(filter);
        }

        /// <summary>
        /// Gerenal deletion of existed model. Selected collection base on property CollectionName.
        /// </summary>
        /// <param name="id">Deleting model id.</param>
        public async Task DeleteAsync(ObjectId id)
        {
            await DeleteAsync(id, CollectionName);
        }

        /// <summary>
        /// General getting of all models.
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TModel>> GetAllAsync(string collectionName)
        {
            var models = await Database.GetCollection<TModel>(collectionName).FindAsync(_ => true);

            return await models.ToListAsync();
        }

        /// <summary>
        /// General getting of all models. Selected collection base on property CollectionName.
        /// </summary>
        /// <returns>IQueryable list of models.</returns>
        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return await GetAllAsync(CollectionName);
        }

        /// <summary>
        /// Gerenal getting of single model by id.
        /// </summary>
        /// <param name="id">Model id.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <returns></returns>
        public async Task<TModel> GetModelAsync(ObjectId id, string collectionName)
        {
            var filter = Builders<TModel>.Filter.Eq("_id", id);

            var result = await Database.GetCollection<TModel>(collectionName).FindAsync(filter);

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Gerenal getting of single model by id. Selected collection base on property CollectionName.
        /// </summary>
        /// <param name="id">Model id.</param>
        /// <returns>Existed model.</returns>
        public async Task<TModel> GetModelAsync(ObjectId id)
        {
            return await GetModelAsync(id, CollectionName);
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

        /// <summary>
        /// Get file by id.
        /// </summary>
        /// <param name="id">,File id.</param>
        /// <returns>Returns bytes of file.</returns>
        public async Task<byte[]> GetFileAsync(ObjectId id)
        {
            var result = await GridFSBucket.DownloadAsBytesAsync(id);

            return result;
        }
    }
}
