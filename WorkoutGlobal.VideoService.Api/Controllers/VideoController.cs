using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using WorkoutGlobal.VideoService.Api.Contracts;
using WorkoutGlobal.VideoService.Api.Filters.ActionFilters;
using WorkoutGlobal.VideoService.Api.Models;
using WorkoutGlobal.VideoService.Api.Models.Dtos;

namespace WorkoutGlobal.VideoService.Api.Controllers
{
    /// <summary>
    /// Represents controller for working with videos.
    /// </summary>
    [Route("api/videos")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private IVideoRepository _videoRepository;
        private IMapper _mapper;

        /// <summary>
        /// Ctor for video controller.
        /// </summary>
        /// <param name="videoRepository">Video repository instance.</param>
        /// <param name="mapper">AutoMapper instance.</param>
        public VideoController(
            IVideoRepository videoRepository, 
            IMapper mapper)
        {
            VideoRepository = videoRepository;
            Mapper = mapper;
        }

        /// <summary>
        /// Authentication repository property.
        /// </summary>
        public IVideoRepository VideoRepository
        {
            get => _videoRepository;
            private set => _videoRepository = value 
                ?? throw new NullReferenceException(nameof(value));
        }

        /// <summary>
        /// AutoMapper instance.
        /// </summary>
        public IMapper Mapper
        {
            get => _mapper;
            private set => _mapper = value 
                ?? throw new NullReferenceException(nameof(value));
        }

        /// <summary>
        /// Get video by id.
        /// </summary>
        /// <param name="id">Video id.</param>
        /// <returns>Returns find video by given id.</returns>
        /// <response code="200">Video was successfully find.</response>
        /// <response code="400">Incoming id isn't valid.</response>
        /// <response code="404">Video with given is not found on server.</response>
        /// <response code="500">Something going wrong on server.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(type: typeof(VideoDto), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVideo(ObjectId id)
        {
            if (id == ObjectId.Empty)
                return BadRequest(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Video id is empty.",
                    Details = "Video id cannot be empty for find action."
                });

            var video = await VideoRepository.GetVideoAsync(id);

            if (video is null)
                return NotFound(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Video not exists.",
                    Details = "Video with given id not found in system."
                });

            var videoDto = Mapper.Map<VideoDto>(video);

            return Ok(videoDto);
        }

        /// <summary>
        /// Get all videos.
        /// </summary>
        /// <returns>Returns collection of all videos.</returns>
        /// <response code="200">Videos was successfully get.</response>
        /// <response code="500">Something going wrong on server.</response>
        [HttpGet]
        [ProducesResponseType(type: typeof(IEnumerable<VideoDto>), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllVideos()
        {
            var videos = await VideoRepository.GetAllVideosAsync();

            var videoDto = Mapper.Map<IEnumerable<VideoDto>>(videos);

            return Ok(videoDto);
        }

        /// <summary>
        /// Create video.
        /// </summary>
        /// <param name="creationVideoDto">Creation model.</param>
        /// <returns></returns>
        /// <response code="201">Video was successfully created.</response>
        /// <response code="400">Incoming model isn't valid.</response>
        /// <response code="500">Something going wrong on server.</response>
        [HttpPost]
        [ModelValidationFilter]
        [ProducesResponseType(type: typeof(string), statusCode: StatusCodes.Status201Created)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateVideo([FromBody] CreationVideoDto creationVideoDto)
        {
            var video = Mapper.Map<Video>(creationVideoDto);

            var createdVideoId = await VideoRepository.CreateVideoAsync(video, creationVideoDto.VideoFile);

            return Created($"api/videos/{createdVideoId}", createdVideoId);
        }

        /// <summary>
        /// Update video.
        /// </summary>
        /// <param name="creationVideoDto">Updation model.</param>
        /// <returns></returns>
        /// <response code="204">Video was successfully deleted.</response>
        /// <response code="400">Incoming model isn't valid.</response>
        /// <response code="500">Something going wrong on server.</response>
        [HttpPut]
        [ModelValidationFilter]
        [ProducesResponseType(type: typeof(int), statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateVideo([FromBody] UpdationVideoDto creationVideoDto)
        {
            var video = Mapper.Map<Video>(creationVideoDto);

            await VideoRepository.UpdateVideoAsync(video);

            return NoContent();
        }

        /// <summary>
        /// Delete video.
        /// </summary>
        /// <param name="id">Deletion id.</param>
        /// <returns></returns>
        /// <response code="204">Video was successfully deleted.</response>
        /// <response code="400">Incoming id isn't valid.</response>
        /// <response code="404">Video with given is not found on server.</response>
        /// <response code="500">Something going wrong on server.</response>
        [HttpDelete]
        [ProducesResponseType(type: typeof(int), statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteVideo(ObjectId id)
        {
            if (id == ObjectId.Empty)
                return BadRequest(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Video id is empty.",
                    Details = "Video id cannot be empty for find action."
                });

            var video = await VideoRepository.GetVideoAsync(id);

            if (video is null)
                return NotFound(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Video not exists.",
                    Details = "Video with given id not found in system."
                });

            await VideoRepository.DeleteVideoAsync(id);

            return NoContent();
        }
    }
}
