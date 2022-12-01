using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WorkoutGlobal.Shared.Messages;
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
    [Produces("application/json")]
    public class VideoController : ControllerBase
    {
        private IVideoRepository _videoRepository;
        private IMapper _mapper;

        /// <summary>
        /// Ctor for video controller.
        /// </summary>
        /// <param name="videoRepository">Video repository instance.</param>
        /// <param name="mapper">AutoMapper instance.</param>
        /// <param name="publisher">Message publisher.</param>
        public VideoController(
            IVideoRepository videoRepository,
            IMapper mapper,
            IPublishEndpoint publisher)
        {
            VideoRepository = videoRepository;
            Mapper = mapper;
            Publisher = publisher;  
        }

        /// <summary>
        /// Service bus.
        /// </summary>
        public IPublishEndpoint Publisher { get; private set; }

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
        public async Task<IActionResult> GetVideo(string id)
        {
            if (!ObjectId.TryParse(id, out var parsedId) || parsedId == ObjectId.Empty)
                return BadRequest(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Video id is invalid.",
                    Details = "Video id cannot be empty for GET action."
                });

            var video = await VideoRepository.GetVideoAsync(parsedId);

            if (video is null)
                return NotFound(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Video not exists.",
                    Details = "Video with given id not found in system."
                });

            var videoDto = Mapper.Map<VideoDto>(video);

            videoDto.VideoFile = await VideoRepository.GetVideoFileAsync(parsedId);

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

            var videosDto = Mapper.Map<IEnumerable<VideoDto>>(videos);

            foreach (var videoDto in videosDto)
                videoDto.VideoFile = await VideoRepository.GetVideoFileAsync(videoDto.Id);

            return Ok(videosDto);
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

            return Created($"api/videos/{createdVideoId}", createdVideoId.ToString());
        }

        // TODO: Нужно полностью пересмотреть концепцию обновления, потому что ничего не понятно
        // как это сделать правильно
        /// <summary>
        /// Update video.
        /// </summary>
        /// <param name="id">Updation model id.</param>
        /// <param name="updationVideoDto">Updation model.</param>
        /// <returns></returns>
        /// <response code="204">Video was successfully deleted.</response>
        /// <response code="400">Incoming model isn't valid.</response>
        /// <response code="500">Something going wrong on server.</response>
        [HttpPut("{id}")]
        [ModelValidationFilter]
        [ProducesResponseType(type: typeof(int), statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateVideo(string id, [FromBody] UpdationVideoDto updationVideoDto)
        {
            if (!ObjectId.TryParse(id, out var parsedId) || parsedId == ObjectId.Empty)
                return BadRequest(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Video id is invalid.",
                    Details = "Video id cannot be empty for UPDATE action."
                });

            var video = await VideoRepository.GetVideoAsync(parsedId);

            if (video is null)
                return NotFound(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Video not exists.",
                    Details = "Video with given id not found in system."
                });

            video = Mapper.Map<Video>(updationVideoDto);
            video.Id = parsedId;

            await VideoRepository.UpdateVideoAsync(video);

            await Publisher.Publish<UpdateVideoMessage>(
                message: new(
                    UpdationId: id,
                    Title: video.Title,
                    Description: video.Description));

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
        [HttpDelete("{id}")]
        [ProducesResponseType(type: typeof(int), statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteVideo(string id)
        {
            if (!ObjectId.TryParse(id, out var parsedId) || parsedId == ObjectId.Empty)
                return BadRequest(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Video id is empty.",
                    Details = "Video id cannot be empty for delete action."
                });

            var video = await VideoRepository.GetVideoAsync(parsedId);

            if (video is null)
                return NotFound(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Video not exists.",
                    Details = "Video with given id not found in system."
                });

            await VideoRepository.DeleteVideoAsync(parsedId);

            await Publisher.Publish<DeleteVideoMessage>(message: new(id));

            return NoContent();
        }

        /// <summary>
        /// Partial update of user info in video models.
        /// </summary>
        /// <param name="updationCreatorId">Creator id.</param>
        /// <param name="patchDocument">Patch document.</param>
        /// <returns></returns>
        /// <response code="204">Video was successfully patched.</response>
        /// <response code="400">Incoming id isn't valid.</response>
        /// <response code="500">Something going wrong on server.</response>
        [HttpPatch("{updationCreatorId}")]
        [ProducesResponseType(type: typeof(int), statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCreator(Guid updationCreatorId, [FromBody] object patchDocument)
        {
            if (patchDocument is null)
                return BadRequest(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Patch document is null",
                    Details = "Patch document for partial updaton of video model is null."
                });

            if (updationCreatorId == Guid.Empty)
                return BadRequest(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Creator account id is empty.",
                    Details = "Id of creator account cannot be empty."
                });

            // only creator name will use
            var updationDto = new UpdationVideoDto();
            // patchDocument.ApplyTo(updationDto);

            var updationModel = Mapper.Map<Video>(updationDto);

            await VideoRepository.UpdateManyAccountVideosAsync(updationCreatorId, updationModel);

            return NoContent();
        }

        /// <summary>
        /// Delete account videos.
        /// </summary>
        /// <param name="deletionAccountId">Deleted account id.</param>
        /// <returns></returns>
        /// <response code="204">Videos was successfully deleted.</response>
        /// <response code="400">Incoming id isn't valid.</response>
        /// <response code="500">Something going wrong on server.</response>
        [HttpDelete("creators/{deletionAccountId}")]
        [ProducesResponseType(type: typeof(int), statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status400BadRequest)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserVideos(Guid deletionAccountId)
        {
            if (deletionAccountId == Guid.Empty)
                return BadRequest(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Creator account id is empty.",
                    Details = "Id of creator account cannot be empty."
                });

            await VideoRepository.DeleteUserVideosAsync(deletionAccountId);

            return NoContent();
        }

        /// <summary>
        /// Purge database after tests.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("purge/{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status404NotFound)]
        [ProducesResponseType(type: typeof(ErrorDetails), statusCode: StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Purge(string id)
        {
            if (!ObjectId.TryParse(id, out var parsedId) || parsedId == ObjectId.Empty)
                return BadRequest(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Video id is empty.",
                    Details = "Video id cannot be empty for delete action."
                });
 
            var video = await VideoRepository.GetVideoAsync(parsedId);

            if (video is null)
                return NotFound(new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Video not exists.",
                    Details = "Video with given id not found in system."
                });

            await VideoRepository.DeleteVideoAsync(parsedId);

            return NoContent();
        }
    }
}
