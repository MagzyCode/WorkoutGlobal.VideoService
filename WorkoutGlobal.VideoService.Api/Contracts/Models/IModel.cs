using MongoDB.Bson;

namespace WorkoutGlobal.VideoService.Api.Contracts
{
    public interface IModel
    {
        public ObjectId Id { get; set; }
    }
}
