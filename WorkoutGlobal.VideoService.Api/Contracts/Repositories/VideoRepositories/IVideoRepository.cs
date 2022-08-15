using MongoDB.Bson;
using WorkoutGlobal.VideoService.Api.Models;

namespace WorkoutGlobal.VideoService.Api.Contracts
{
    /// <summary>
    /// Base interface for video repository.
    /// </summary>
    public interface IVideoRepository
    {
        /// <summary>
        /// Get all videos.
        /// </summary>
        /// <returns>Return collection of all videos.</returns>
        public Task<IEnumerable<Video>> GetAllVideosAsync();

        /// <summary>
        /// Get video by id.
        /// </summary>
        /// <param name="objectId">Video id.</param>
        /// <returns>Returns find video by given id.</returns>
        public Task<Video> GetVideoAsync(ObjectId objectId);

        /// <summary>
        /// Create video.
        /// </summary>
        /// <param name="creationVideo">Creation video model.</param>
        /// <param name="videoFile">Loaded video file.</param>
        /// <returns>Returns generated id for creation video.</returns>
        public Task<ObjectId> CreateVideoAsync(Video creationVideo, IFormFile videoFile);

        /// <summary>
        /// Update video.
        /// </summary>
        /// <param name="updationVideo">Updation video model.</param>
        /// <returns></returns>
        public Task UpdateVideoAsync(Video updationVideo);

        /// <summary>
        /// Delete video.
        /// </summary>
        /// <param name="deletionId">Deletion video id.</param>
        /// <returns></returns>
        public Task DeleteVideoAsync(ObjectId deletionId);
    }
}
