using MongoDB.Bson;
using WorkoutGlobal.VideoService.Api.Models;

namespace WorkoutGlobal.VideoService.Api.Contracts
{
    public interface IVideoRepository
    {
        public Task<IEnumerable<Video>> GetAllVideosAsync();

        public Task<Video> GetVideoAsync(ObjectId objectId);

        public Task<ObjectId> CreateVideoAsync(Video creationVideo);

        public Task UpdateVideoAsync(Video updationVideo);

        public Task DeleteVideoAsync(ObjectId deletionId);
    }
}
