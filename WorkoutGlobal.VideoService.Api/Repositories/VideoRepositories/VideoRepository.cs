using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using WorkoutGlobal.VideoService.Api.Contracts;
using WorkoutGlobal.VideoService.Api.Models;
using static MongoDB.Driver.WriteConcern;
// using static MongoDB.Driver.WriteConcern;

namespace WorkoutGlobal.VideoService.Api.Repositories
{
    /// <summary>
    /// Base repository for video manipulation.
    /// </summary>
    public class VideoRepository : BaseRepository<Video>, IVideoRepository
    {
        /// <summary>
        /// Ctor for video repository.
        /// </summary>
        /// <param name="configuration">Project configuration.</param>
        /// <param name="collectionName">Base collection name.</param>
        public VideoRepository(IConfiguration configuration, string collectionName = "Videos")
            : base(configuration, collectionName)
        {   }

        /// <summary>
        /// Create video.
        /// </summary>
        /// <param name="creationVideo">Creation video model.</param>
        /// <param name="videoFile">Loaded video file.</param>
        /// <returns>Returns generated id for creation video.</returns>
        public async Task<ObjectId> CreateVideoAsync(Video creationVideo, byte[] videoFile)
        {
            if (creationVideo is null)
                throw new ArgumentNullException(nameof(creationVideo), "Video model cannot be null");

            if (videoFile is null || videoFile.Length == 0)
                throw new ArgumentNullException(nameof(videoFile), "Video file cannot be null");
          
            var createdGridFSId = await AddFileAsync(
                videoName: creationVideo.FileName,
                videoFile: videoFile);

            creationVideo.GridFsId = createdGridFSId;
            var createdId = await CreateAsync(creationVideo);

            return createdId;
        }

        /// <summary>
        /// Delete video.
        /// </summary>
        /// <param name="deletionId">Deletion video id.</param>
        /// <returns></returns>
        public async Task DeleteVideoAsync(ObjectId deletionId)
        {
            var deletedVideo = await GetVideoAsync(deletionId);

            await GridFSBucket.DeleteAsync(deletedVideo.GridFsId);

            await DeleteAsync(deletionId);
        }

        /// <summary>
        /// Get all videos.
        /// </summary>
        /// <returns>Return collection of all videos.</returns>
        public async Task<IEnumerable<Video>> GetAllVideosAsync()
        {
            var videos = await GetAllAsync();

            return videos;
        }

        /// <summary>
        /// Get video by id.
        /// </summary>
        /// <param name="objectId">Video id.</param>
        /// <returns>Returns find video by given id.</returns>
        public async Task<Video> GetVideoAsync(ObjectId objectId)
        {
            var video = await GetModelAsync(objectId);

            return video;
        }

        /// <summary>
        /// Get video file by video id.
        /// </summary>
        /// <param name="id">Video file id.</param>
        /// <returns>Return bytes of video file.</returns>
        public async Task<byte[]> GetVideoFileAsync(ObjectId id)
        {
            var video = await GetVideoAsync(id);

            var resultFile = await GetFileAsync(video.GridFsId);

            return resultFile;
        }

        /// <summary>
        /// Update video.
        /// </summary>
        /// <param name="updationVideo">Updation video model.</param>
        /// <returns></returns>
        public async Task UpdateVideoAsync(Video updationVideo)
        {
            if (updationVideo is null)
                throw new ArgumentNullException(nameof(updationVideo), "Updation video cannot be null.");

            var filter = Builders<Video>.Filter.Eq("_id", updationVideo.Id);

            var update = Builders<Video>.Update
                .Set(video => video.Title, updationVideo.Title)
                .Set(video => video.Description, updationVideo.Description)
                .Set(video => video.CreatorId, updationVideo.CreatorId)
                .Set(video => video.CreatorFullName, updationVideo.CreatorFullName)
                .Set(video => video.FileName, updationVideo.FileName);

            await Database.GetCollection<Video>(CollectionName).UpdateOneAsync(
                filter: filter,
                update: update);
        }

        /// <summary>
        /// Partial update of user info in video database.
        /// </summary>
        /// <param name="creatorAccountId">Updated account id.</param>
        /// <param name="updationModel">Updated model.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws if account id is empty.</exception>
        /// <exception cref="ArgumentNullException">Throws if account name is null or empty.</exception>
        public async Task UpdateManyAccountVideosAsync(Guid creatorAccountId, Video updationModel)
        {
            if (creatorAccountId == Guid.Empty)
                throw new ArgumentException("Creator id cannot be empty", nameof(creatorAccountId));

            if (updationModel is null)
                throw new ArgumentNullException(nameof(updationModel), "Creator name cannot be null or empty");

            var filter = Builders<Video>.Filter.Eq("CreatorId", creatorAccountId);

            var updateManyQuery = Builders<Video>.Update
                .Set(video => video.CreatorFullName, updationModel.CreatorFullName);

            await Database.GetCollection<Video>(CollectionName).UpdateManyAsync(
                filter: filter,
                update: updateManyQuery);
        }

        /// <summary>
        /// Delete all deleted user videos.
        /// </summary>
        /// <param name="userAccountId">Deletion account id.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws id creator id is empty.</exception>
        public async Task DeleteUserVideosAsync(Guid userAccountId)
        {
            if (userAccountId == Guid.Empty)
                throw new ArgumentException("Creator id cannot be empty", nameof(userAccountId));

            var filter = Builders<Video>.Filter.Eq("CreatorId", userAccountId);

            await Database.GetCollection<Video>(CollectionName).DeleteManyAsync(filter);
        }
    }
}
