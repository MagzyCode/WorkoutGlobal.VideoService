using WorkoutGlobal.VideoService.Api.Contracts;
using WorkoutGlobal.VideoService.Api.Repositories;

namespace WorkoutGlobal.VideoService.Api.Extensions
{
    /// <summary>
    /// Base class for all service extensions.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Configure instances of repository classes.
        /// </summary>
        /// <param name="services">Project services.</param>
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IVideoRepository, VideoRepository>();
        }
    }
}
