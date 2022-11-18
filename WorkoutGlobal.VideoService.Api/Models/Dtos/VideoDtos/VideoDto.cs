using MongoDB.Bson;

namespace WorkoutGlobal.VideoService.Api.Models.Dtos
{
    /// <summary>
    /// Represents video DTO model for GET method.
    /// </summary>
    public class VideoDto
    {
        /// <summary>
        /// Unique identifier of video.
        /// </summary>
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
        /// Video file name;
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Creator id.
        /// </summary>
        public Guid CreatorId { get; set; }

        /// <summary>
        /// Creator full name
        /// </summary>
        public string CreatorFullName { get; set; }

        /// <summary>
        /// Video file in bytes.
        /// </summary>
        public byte[] VideoFile { get; set; }
    }
}
