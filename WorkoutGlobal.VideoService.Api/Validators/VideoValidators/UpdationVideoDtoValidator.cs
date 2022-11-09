using FluentValidation;
using WorkoutGlobal.VideoService.Api.Models.Dtos;

namespace WorkoutGlobal.VideoService.Api.Validators
{
    /// <summary>
    /// Validator for updation video DTO model.
    /// </summary>
    public class UpdationVideoDtoValidator : AbstractValidator<UpdationVideoDto>
    {
        /// <summary>
        /// Ctor for updation video validator.
        /// </summary>
        public UpdationVideoDtoValidator()
        {
            RuleFor(video => video.Title)
                .NotEmpty();

            RuleFor(video => video.Description)
                .NotEmpty();

            RuleFor(video => video.FileName)
                .NotEmpty();
        }
    }
}
