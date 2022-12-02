using MassTransit;
using WorkoutGlobal.VideoService.Api.Contracts;
using WorkoutGlobal.VideoService.Api.Filters.ActionFilters;
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

        /// <summary>
        /// Configure instances of attributes.
        /// </summary>
        /// <param name="services">Project services.</param>
        public static void ConfigureAttributes(this IServiceCollection services)
        {
            services.AddScoped<ModelValidationFilterAttribute>();
        }

        /// <summary>
        /// Configure MassTransit.
        /// </summary>
        /// <param name="services">Project services.</param>
        /// <param name="configuration">Project configuration.</param>
        public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(options =>
            {
                options.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(configuration["MassTransitSettings:Host"]);
                });
            });
            services.AddMassTransitHostedService();
        }
    }
}
