using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WorkoutGlobal.VideoService.Api.Contracts;

namespace WorkoutGlobal.VideoService.Api.Models
{
    
    /// <summary>
    /// Represents model of video files.
    /// </summary>
    public class Video : IModel
    {
        /// <summary>
        /// Unique identifier of video.
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Video title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Video description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Identifier of video in GridFS.
        /// </summary>
        public ObjectId GridFsId { get; set; }
    }
}
