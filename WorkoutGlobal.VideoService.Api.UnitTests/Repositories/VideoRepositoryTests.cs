using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using WorkoutGlobal.VideoService.Api.Models;
using WorkoutGlobal.VideoService.Api.Repositories;
using WorkoutGlobal.VideoService.Api.UnitTests.Configuration;

namespace WorkoutGlobal.VideoService.Api.UnitTests.Repositories
{
    public class VideoRepositoryTests
    {
        private readonly VideoRepository _videoRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public VideoRepositoryTests()
        {
            _mockConfiguration = new();
            _mockConfiguration
                .Setup(x => x[It.IsAny<string>()])
                .Returns("mongodb://localhost:11111");

            _videoRepository = new VideoRepository(_mockConfiguration.Object);
        }

        [Fact]
        public async Task CreateVideoAsync_NullCreationVideoParam_ReturnArgumentNullException()
        {
            // arrange
            Video creationVideo = null;

            // act
            var result = async () => await _videoRepository.CreateVideoAsync(creationVideo, null);

            // assert
            await result.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateVideoAsync_NullVideoFileParam_ReturnArgumentNullException()
        {
            // arrange
            byte[] videoFile = null;

            // act
            var result = async () => await _videoRepository.CreateVideoAsync(null, videoFile);

            // assert
            await result.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateVideoAsync_NullUpdationVideoParam_ReturnArgumentNullException()
        {
            // arrange
            Video updationVideo = null;

            // act
            var result = async () => await _videoRepository.UpdateVideoAsync(updationVideo);

            // assert
            await result.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
