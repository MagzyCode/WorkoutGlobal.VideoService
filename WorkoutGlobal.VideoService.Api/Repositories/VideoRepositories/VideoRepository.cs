using MongoDB.Bson;
using WorkoutGlobal.VideoService.Api.Contracts;
using WorkoutGlobal.VideoService.Api.Models;

namespace WorkoutGlobal.VideoService.Api.Repositories
{
    public class VideoRepository : BaseRepository<Video>, IVideoRepository
    {
        public VideoRepository(IConfiguration configuration, string collectionName)
            : base(configuration, collectionName = "Videos")
        { }

        public async Task<ObjectId> CreateVideoAsync(Video creationVideo)
        {
            var createdId = await CreateAsync(creationVideo);

            return createdId;
        }

        public async Task DeleteVideoAsync(ObjectId deletionId)
        {
            await DeleteAsync(deletionId);
        }

        public async Task<IEnumerable<Video>> GetAllVideosAsync()
        {
            var videos = await GetAllAsync();

            return videos;
        }

        public async Task<Video> GetVideoAsync(ObjectId objectId)
        {
            var video = await GetModelAsync(objectId);

            return video;
        }

        public async Task UpdateVideoAsync(Video updationVideo)
        {
            await UpdateAsync(updationVideo);
        }
    }
}
