using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace WorkoutGlobal.VideoService.Api.Contracts
{
    /// <summary>
    /// Base interface for all repositories.
    /// </summary>
    /// <typeparam name="TModel">Model class.</typeparam>
    public interface IBaseRepository<TModel>
    {

        public string CollectionName { get; set; }
        public IMongoDatabase Database { get; set; }

        /// <summary>
        /// Project configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

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
        /// <param name="model">Deleting model.</param>
        public Task DeleteAsync(ObjectId model);

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
    }
}
