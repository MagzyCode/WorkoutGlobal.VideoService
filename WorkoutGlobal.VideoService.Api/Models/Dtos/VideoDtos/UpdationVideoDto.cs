using MongoDB.Bson;

namespace WorkoutGlobal.VideoService.Api.Models.Dtos
{
    /// <summary>
    /// Video DTO model for UPDATE method.
    /// </summary>
    public class UpdationVideoDto
    {
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
    }
}
