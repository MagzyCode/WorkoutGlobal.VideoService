using WorkoutGlobal.VideoService.Api.Middlewares;

namespace WorkoutGlobal.VideoService.Api.Extensions
{
    /// <summary>
    /// Base class for all middleware extensions.
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Add global exception handler middleware in request pipeline.
        /// </summary>
        /// <param name="app">Application builder instance.</param>
        /// <returns>Application builder.</returns>
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
