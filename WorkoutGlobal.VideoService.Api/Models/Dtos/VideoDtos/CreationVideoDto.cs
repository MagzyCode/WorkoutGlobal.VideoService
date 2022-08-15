namespace WorkoutGlobal.VideoService.Api.Models.Dtos
{
    /// <summary>
    /// Represents video DTO model for POST method.
    /// </summary>
    public class CreationVideoDto
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

        /// <summary>
        /// Image file bytes.
        /// </summary>
        public IFormFile VideoFile { get; set; }
    }
}
