using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutGlobal.VideoService.Api.Contracts;
using WorkoutGlobal.VideoService.Api.Controllers;
using WorkoutGlobal.VideoService.Api.Models;
using WorkoutGlobal.VideoService.Api.Models.Dtos;

namespace WorkoutGlobal.VideoService.Api.UnitTests.Controllers
{
    public class VideoControllerTests
    {
        private readonly VideoController _videoController;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly Mock<IMapper> _mapperMoq;

        public VideoControllerTests()
        {
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _videoRepositoryMock
                .Setup(x => x.GetVideoAsync(It.IsAny<ObjectId>()))
                .ReturnsAsync(new Video());
            _videoRepositoryMock
                .Setup(x => x.CreateVideoAsync(It.IsAny<Video>(), It.IsAny<IFormFile>()))
                .ReturnsAsync(ObjectId.Empty);
            _videoRepositoryMock
                .Setup(x => x.UpdateVideoAsync(It.IsAny<Video>()));            
            _videoRepositoryMock
                .Setup(x => x.DeleteVideoAsync(It.IsAny<ObjectId>()));
                

            _mapperMoq = new Mock<IMapper>();
            _mapperMoq
                .Setup(x => x.Map<VideoDto>(It.IsAny<Video>()))
                .Returns(new VideoDto());
            _mapperMoq
                .Setup(x => x.Map<IEnumerable<VideoDto>>(It.IsAny<IEnumerable<Video>>()))
                .Returns(new List<VideoDto>());
            _mapperMoq
                .Setup(x => x.Map<Video>(It.IsAny<CreationVideoDto>()))
                .Returns(new Video());
            _mapperMoq
                .Setup(x => x.Map<Video>(It.IsAny<UpdationVideoDto>()))
                .Returns(new Video());

            _videoController = new VideoController(
                videoRepository: _videoRepositoryMock.Object,
                mapper: _mapperMoq.Object);
        }

        [Fact]
        public async Task GetVideo_InvalidId_ReturnBadRequestResult()
        {
            // arrange
            var id = ObjectId.Empty;

            // act 
            var result = await _videoController.GetVideo(id);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequestResult = result.As<BadRequestObjectResult>();
            badRequestResult.Value.Should().NotBeNull();
            badRequestResult.Value.Should().BeOfType<ErrorDetails>();
            
            var error = badRequestResult.Value.As<ErrorDetails>();
            error.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            error.Message.Should().Be("Video id is empty.");
            error.Details.Should().Be("Video id cannot be empty for find action.");
        }

        [Fact]
        public async Task GetVideo_VideoNotFound_ReturnNotFoundResult()
        {
            // arrange
            var id = ObjectId.GenerateNewId();
            _videoRepositoryMock
                .Setup(x => x.GetVideoAsync(id)); 

            // act 
            var result = await _videoController.GetVideo(id);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();

            var notFoundResult = result.As<NotFoundObjectResult>();
            notFoundResult.Value.Should().NotBeNull();
            notFoundResult.Value.Should().BeOfType<ErrorDetails>();

            var error = notFoundResult.Value.As<ErrorDetails>();
            error.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            error.Message.Should().Be("Video not exists.");
            error.Details.Should().Be("Video with given id not found in system.");
        }

        [Fact]
        public async Task GetVideo_ModelExists_ReturnNotFoundResult()
        {
            // arrange
            var id = ObjectId.GenerateNewId();

            // act 
            var result = await _videoController.GetVideo(id);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().NotBeNull();
            okResult.Value.Should().BeOfType<VideoDto>();
        }

        [Fact]
        public async Task GetAllVideos_ValidState_ReturnsOkObjectResult()
        {
            // arrange
            _videoRepositoryMock
                .Setup(x => x.GetAllVideosAsync())
                .ReturnsAsync(new List<Video>());

            // act
            var result = await _videoController.GetAllVideos();

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().NotBeNull();
            okResult.Value.Should().BeOfType<List<VideoDto>>();
        }

        [Fact]
        public async Task CreateVideo_ValidModel_ReturnsCreatedObject()
        {
            // arrange
            var creationVideoDto = new CreationVideoDto();
            
            // act
            var result = await _videoController.CreateVideo(creationVideoDto);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CreatedResult>();

            var createdResult = result.As<CreatedResult>();
            createdResult.Value.Should().NotBeNull();
            createdResult.Value.Should().BeOfType<ObjectId>();
            createdResult.Value.Should().Be(ObjectId.Empty);
            createdResult.Location.Should().NotBeNullOrEmpty();
            createdResult.Location.Should().BeOfType<string>();
            createdResult.Location.Should().Be($"api/videos/{ObjectId.Empty}");
        }

        [Fact]
        public async Task UpdateVideo_ValidState_ReturnsNoContentObject()
        {
            // arrange
            var updationVideoDto = new UpdationVideoDto();

            // act
            var result = await _videoController.UpdateVideo(updationVideoDto);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteVideo_InvalidId_ReturnsBadRequestObject()
        {
            // arrange
            var id = ObjectId.Empty;

            // act
            var result = await _videoController.DeleteVideo(id);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequestResult = result.As<BadRequestObjectResult>();
            badRequestResult.Value.Should().NotBeNull();
            badRequestResult.Value.Should().BeOfType<ErrorDetails>();

            var error = badRequestResult.Value.As<ErrorDetails>();
            error.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            error.Message.Should().Be("Video id is empty.");
            error.Details.Should().Be("Video id cannot be empty for delete action.");
        }

        [Fact]
        public async Task DeleteVideo_ModelNotExists_ReturnsNotFoundObject()
        {
            // arrange
            var id = ObjectId.GenerateNewId();
            _videoRepositoryMock
                .Setup(x => x.GetVideoAsync(id));

            // act
            var result = await _videoController.DeleteVideo(id);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();

            var notFoundResult = result.As<NotFoundObjectResult>();
            notFoundResult.Value.Should().NotBeNull();
            notFoundResult.Value.Should().BeOfType<ErrorDetails>();

            var error = notFoundResult.Value.As<ErrorDetails>();
            error.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            error.Message.Should().Be("Video not exists.");
            error.Details.Should().Be("Video with given id not found in system.");
        }

        [Fact]
        public async Task DeleteVideo_ValidState_ReturnsNoContentObject()
        {
            // arrange
            var id = ObjectId.GenerateNewId();

            // act
            var result = await _videoController.DeleteVideo(id);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
