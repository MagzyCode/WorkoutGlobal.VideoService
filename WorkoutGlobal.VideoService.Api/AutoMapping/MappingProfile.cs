using AutoMapper;
using WorkoutGlobal.VideoService.Api.Models;
using WorkoutGlobal.VideoService.Api.Models.Dtos;

namespace WorkoutGlobal.VideoService.Api.AutoMapping
{
    /// <summary>
    /// Class for configure mapping rules via AutoMapper library.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Ctor for set mapping rules for models and DTOs.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<Video, VideoDto>();
            CreateMap<CreationVideoDto, Video>();
        }
    }
}
