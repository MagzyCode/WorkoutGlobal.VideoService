using AutoFixture;
using FluentAssertions;
using FluentValidation.Results;
using WorkoutGlobal.VideoService.Api.Models.Dtos;
using WorkoutGlobal.VideoService.Api.Validators.VideoValidators;

namespace WorkoutGlobal.VideoService.Api.UnitTests.Validators
{
    public class CreationVideoDtoValidatorTests
    {
        private readonly CreationVideoDtoValidator _validator = new();
        private readonly Fixture _fixture = new();

        [Fact]
        public async Task ModelState_NullProperties_ReturnValidationResult()
        {
            // arrange 
            var creationVideoDto = new CreationVideoDto();

            // act
            var validationResult = await _validator.ValidateAsync(creationVideoDto);

            // assert
            validationResult.Should().BeOfType(typeof(ValidationResult));
            validationResult.Should().NotBeNull();
            validationResult.Errors.Should().HaveCount(4);
            validationResult.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task ModelState_EmptyProperties_ReturnValidationResult()
        {
            // arrange 
            var creationVideoDto = _fixture.Build<CreationVideoDto>()
                .OmitAutoProperties()
                .With(video => video.Title, string.Empty)
                .With(video => video.Description, string.Empty)
                .With(video => video.FileName, string.Empty)
                .Create();

            // act
            var validationResult = await _validator.ValidateAsync(creationVideoDto);

            // assert
            validationResult.Should().BeOfType(typeof(ValidationResult));
            validationResult.Should().NotBeNull();
            validationResult.Errors.Should().HaveCount(4);
            validationResult.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task ModelState_ValidProperties_ReturnValidationResult()
        {
            // arrange 
            var creationVideoDto = _fixture.Build<CreationVideoDto>()
                .Without(video => video.VideoFile)
                .Create();

            // act
            var validationResult = await _validator.ValidateAsync(creationVideoDto);

            // assert
            validationResult.Should().BeOfType(typeof(ValidationResult));
            validationResult.Should().NotBeNull();
            validationResult.Errors.Should().HaveCount(1);
        }
    }
}
