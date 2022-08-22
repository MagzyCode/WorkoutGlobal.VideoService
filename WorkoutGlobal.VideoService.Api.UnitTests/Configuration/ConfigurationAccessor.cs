using Microsoft.Extensions.Configuration;

namespace WorkoutGlobal.VideoService.Api.UnitTests.Configuration
{
    public class ConfigurationAccessor
    {
        public static IConfiguration GetTestConfiguration(string settingFilePath = "appsettings.json")
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(settingFilePath, optional: false, reloadOnChange: true)
                .Build();

            return configuration;
        }
    }
}
