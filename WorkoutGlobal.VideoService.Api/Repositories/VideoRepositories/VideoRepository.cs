using MongoDB.Bson;
using WorkoutGlobal.VideoService.Api.Contracts;
using WorkoutGlobal.VideoService.Api.Models;

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
        public async Task<ObjectId> CreateVideoAsync(Video creationVideo, IFormFile videoFile)
        {
            if (creationVideo is null)
                throw new ArgumentNullException(nameof(creationVideo), "Video model cannot be null");

            if (videoFile is null)
                throw new ArgumentNullException(nameof(videoFile), "Video file cannot be null");
            
            using var binaryReader = new BinaryReader(videoFile.OpenReadStream());
            var videoData = binaryReader.ReadBytes((int)videoFile.Length);

            var createdGridFSId = await AddFileAsync(
                videoName: creationVideo.FileName,
                videoFile: videoData);

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
        /// Update video.
        /// </summary>
        /// <param name="updationVideo">Updation video model.</param>
        /// <returns></returns>
        public async Task UpdateVideoAsync(Video updationVideo)
        {
            if (updationVideo is null)
                throw new ArgumentNullException(nameof(updationVideo), "Video model cannot be null");

            await UpdateAsync(updationVideo);
        }
    }
}
