using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WorkoutGlobal.VideoService.Api.Contracts;
using WorkoutGlobal.VideoService.Api.Models;

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

        public VideoController(IVideoRepository videoRepository)
        {
            VideoRepository = videoRepository;
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

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetVideo(ObjectId id)
        //{
        //    if (id == ObjectId.Empty)
        //        return BadRequest(new ErrorDetails()
        //        {
        //            StatusCode = StatusCodes.Status400BadRequest,
        //            Message = "Video id is empty.",
        //            Details = "Video id cannot be empty for find action."
        //        });

        //    var video = await VideoRepository.GetVideoAsync(id);

        //    if (video is null)
        //        return NotFound(new ErrorDetails()
        //        {
        //            StatusCode = StatusCodes.Status404NotFound,
        //            Message = "Video not exists.",
        //            Details = "Video with given id not found in system."
        //        });


        //}


    }
}
