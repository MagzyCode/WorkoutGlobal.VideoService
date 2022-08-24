using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace WorkoutGlobal.VideoService.Api.Contracts
{
    /// <summary>
    /// Base interface for all repositories.
    /// </summary>
    /// <typeparam name="TModel">Model class.</typeparam>
    public interface IBaseRepository<TModel>
    {
        /// <summary>
        /// Collection name.
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Current used database.
        /// </summary>
        public IMongoDatabase Database { get; set; }

        /// <summary>
        /// Project configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// GridFS instance.
        /// </summary>
        public IGridFSBucket GridFSBucket { get; }

        /// <summary>
        /// Gerenal creation of new model.
        /// </summary>
        /// <param name="model">Creation model.</param>
        /// <returns>A task that represents asynchronous Create operation.</returns>
        public Task<ObjectId> CreateAsync(TModel model);

        /// <summary>
        /// General update action for existed model.
        /// </summary>
        /// <param name="model">Updated model.</param>
        public Task UpdateAsync(TModel model);

        /// <summary>
        /// Gerenal deletion of existed model.
        /// </summary>
        /// <param name="id">Deleting model.</param>
        public Task DeleteAsync(ObjectId id);

        /// <summary>
        /// General getting of all models.
        /// </summary>
        /// <returns>IQueryable list of models.</returns>
        public Task<IEnumerable<TModel>> GetAllAsync();

        /// <summary>
        /// Gerenal getting of single model by id.
        /// </summary>
        /// <param name="id">Model id.</param>
        /// <returns>Existed model.</returns>
        public Task<TModel> GetModelAsync(ObjectId id);

        /// <summary>
        /// Add file in GridFS.
        /// </summary>
        /// <param name="videoName">Video file name.</param>
        /// <param name="videoFile">Video file data.</param>
        /// <returns>Returns generated id for file.</returns>
        public Task<ObjectId> AddFileAsync(string videoName, byte[] videoFile);

        /// <summary>
        /// Get file by id.
        /// </summary>
        /// <param name="id">,File id.</param>
        /// <returns>Returns bytes of file.</returns>
        public Task<byte[]> GetFileAsync(ObjectId id);
    }
}
