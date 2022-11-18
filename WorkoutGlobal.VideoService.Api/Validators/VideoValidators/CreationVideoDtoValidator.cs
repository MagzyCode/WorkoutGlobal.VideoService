using FluentValidation;
using WorkoutGlobal.VideoService.Api.Models.Dtos;

namespace WorkoutGlobal.VideoService.Api.Validators.VideoValidators
{
    /// <summary>
    /// Validator for creation video DTO model.
    /// </summary>
    public class CreationVideoDtoValidator : AbstractValidator<CreationVideoDto>
    {
        /// <summary>
        /// Ctor for creation video validator.
        /// </summary>
        public CreationVideoDtoValidator()
        {
            RuleFor(video => video.Title)
                .NotEmpty();

            RuleFor(video => video.Description)
                .NotEmpty();

            RuleFor(video => video.FileName)
                .NotEmpty();

            RuleFor(video => video.VideoFile)
                .NotEmpty();

            RuleFor(video => video.CreatorId)
                .NotEmpty();

            RuleFor(video => video.CreatorFullName)
                .NotEmpty();
        }
    }
}
