using MongoDB.Bson;

namespace WorkoutGlobal.VideoService.Api.Contracts
{
    /// <summary>
    /// Base interface for models.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Unique model id.
        /// </summary>
        public ObjectId Id { get; set; }
    }
}
