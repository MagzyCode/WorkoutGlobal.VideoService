using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Net;
using System.Net.Http.Json;
using WorkoutGlobal.VideoService.Api.Models;
using WorkoutGlobal.VideoService.Api.Models.Dtos;

namespace WorkoutGlobal.VideoService.Api.IntegrationTests.Controllers
{
    public class VideoControllerIntegrationTests : IAsyncLifetime
    {
        private readonly AppTestConnection<string> _appTestConnection;
        private readonly Fixture _fixture = new();
        private CreationVideoDto _creationModel;

        public VideoControllerIntegrationTests()
        {
            _appTestConnection = new();
        }

        public async Task InitializeAsync()
        {
            _appTestConnection.PurgeList.Clear();

            var videoFile = await File.ReadAllBytesAsync(_appTestConnection.Configuration["TestVideoPath"]);

            _creationModel = _fixture.Build<CreationVideoDto>()
                .With(video => video.Title, "First video title")
                .With(video => video.Description, "First description")
                .With(video => video.FileName, "firstVideo.mp4")
                .With(video => video.VideoFile, videoFile)
                .Create();
        }

        public async Task DisposeAsync()
        {
            foreach (var id in _appTestConnection.PurgeList)
                _ = await _appTestConnection.AppClient.DeleteAsync($"api/videos/purge/{id}");
                 
            await Task.CompletedTask;
        }

        [Fact]
        public async Task CreateVideo_ValidState_ReturnCreatedResult()
        {
            // arrange
            var createUri = "api/videos";

            // act
            var createResponse = await _appTestConnection.AppClient.PostAsJsonAsync(createUri, _creationModel);
            var result = await createResponse.Content.ReadFromJsonAsync<string>();
            _appTestConnection.PurgeList.Add(result);

            // assert
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<string>();
            ObjectId.TryParse(result, out _).Should().BeTrue();
        }

        [Fact]
        public async Task CreateVideo_NullModel_ReturnCreatedResult()
        {
            // arrange
            _creationModel = null;

            // act
            var createResponse = await _appTestConnection.AppClient.PostAsJsonAsync("api/videos", _creationModel);
            var result = await createResponse.Content.ReadFromJsonAsync<ErrorDetails>();

            // assert
            createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<ErrorDetails>();
            result.Message.Should().Be("Incoming DTO model is null.");
            result.Details.Should().Be("Incoming DTO model not contain any value.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateVideo_InvalidModel_ReturnCreatedResult()
        {
            // arrange
            _creationModel = _fixture.Build<CreationVideoDto>()
                .OmitAutoProperties()
                .Create();

            // act
            var createResponse = await _appTestConnection.AppClient.PostAsJsonAsync("api/videos", _creationModel);
            var result = await createResponse.Content.ReadFromJsonAsync<ErrorDetails>();

            // assert
            createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<ErrorDetails>();
            result.Message.Should().Be("Dto model isn't valid.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }


        [Fact]
        public async Task GetVideo_ExistedVideo_ReturnVideo()
        {
            // arrange
            var createResponse = await _appTestConnection.AppClient.PostAsJsonAsync("api/videos", _creationModel);
            var createdId = await createResponse.Content.ReadFromJsonAsync<string>();
            _appTestConnection.PurgeList.Add(createdId);

            // act
            var getResponse = await _appTestConnection.AppClient.GetAsync($"api/videos/{createdId}");
            var result = await getResponse.Content.ReadFromJsonAsync<VideoDto>();

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<VideoDto>();
            result.Title.Should().Be("First video title");
            result.Description.Should().Be("First description");
            result.FileName.Should().Be("firstVideo.mp4");
            result.VideoFile.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetAllVideo_ExistedVideos_ReturnVideos()
        {
            // arrange
            var createResponse = await _appTestConnection.AppClient.PostAsJsonAsync("api/videos", _creationModel);
            var createdId = await createResponse.Content.ReadFromJsonAsync<string>();
            _appTestConnection.PurgeList.Add(createdId);

            // act
            var getResponse = await _appTestConnection.AppClient.GetAsync($"api/videos");
            var result = await getResponse.Content.ReadFromJsonAsync<List<VideoDto>>();

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<VideoDto>>();
            result.Count.Should().BeGreaterThanOrEqualTo(1);
        }

        [Fact]
        public async Task DeleteVideo_ValidDeletingId_ReturnNoContentResult()
        {
            // arrange
            var createResponse = await _appTestConnection.AppClient.PostAsJsonAsync("api/videos", _creationModel);
            var createdId = await createResponse.Content.ReadFromJsonAsync<string>();
            _appTestConnection.PurgeList.Add(createdId);

            // act
            var deleteResponse = await _appTestConnection.AppClient.DeleteAsync($"api/videos/{createdId}");
            var getResponse = await _appTestConnection.AppClient.GetAsync($"api/videos/{createdId}");
            var getResult = await getResponse.Content.ReadFromJsonAsync<ErrorDetails>();

            // assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            getResult.Should().NotBeNull();
            getResult.Should().BeOfType<ErrorDetails>();
            getResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            getResult.Message.Should().Be("Video not exists.");
            getResult.Details.Should().Be("Video with given id not found in system.");
        }

        [Fact]
        public async Task UpdateVideo_NullDtoModel_ReturnBadRequestResult()
        {
            // arrange
            UpdationVideoDto updationVideoDto = null;

            // act
            var updateResponse = await _appTestConnection.AppClient.PutAsJsonAsync($"api/videos/{ObjectId.Empty}", updationVideoDto);
            var result = await updateResponse.Content.ReadFromJsonAsync<ErrorDetails>();

            // assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<ErrorDetails>();
            result.Message.Should().Be("Incoming DTO model is null.");
            result.Details.Should().Be("Incoming DTO model not contain any value.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task UpdateVideo_InvalidDtoModel_ReturnBadRequestResult()
        {
            // arrange
            var updationModel = _fixture.Build<UpdationVideoDto>()
                .OmitAutoProperties()
                .Create();

            // act
            var updateResponse = await _appTestConnection.AppClient.PutAsJsonAsync($"api/videos/{ObjectId.Empty}", updationModel);
            var result = await updateResponse.Content.ReadFromJsonAsync<ErrorDetails>();

            // assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            result.Should().NotBeNull();
            result.Should().BeOfType<ErrorDetails>();
            result.Message.Should().Be("Dto model isn't valid.");
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task UpdateVideo_ExistedModel_ReturnBadRequestResult()
        {
            // arrange
            var createResponse = await _appTestConnection.AppClient.PostAsJsonAsync("api/videos", _creationModel);
            var createdId = await createResponse.Content.ReadFromJsonAsync<string>();
            _appTestConnection.PurgeList.Add(createdId);
            var updationModel = _fixture.Build<UpdationVideoDto>()
                    .With(video => video.Title, "Update for new title")
                    .With(video => video.Description, "Update for new description")
                    .With(video => video.FileName, "newFile.mp4")
                    .Create();

            // act
            var updateResponse = await _appTestConnection.AppClient.PutAsJsonAsync($"api/videos/{createdId}", updationModel);
            var getResponse = await _appTestConnection.AppClient.GetAsync($"api/videos/{createdId}");
            var getResult = await getResponse.Content.ReadFromJsonAsync<VideoDto>();

            // assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            getResult.Should().NotBeNull();
            getResult.Should().BeOfType<VideoDto>();
            getResult.Title.Should().Be("Update for new title");
            getResult.Description.Should().Be("Update for new description");
            getResult.FileName.Should().Be("newFile.mp4");
        }
    }
}
