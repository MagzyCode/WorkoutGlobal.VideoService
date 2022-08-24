using Microsoft.Extensions.Configuration;
using WorkoutGlobal.VideoService.Api.IntegrationTests.Configuration;

namespace WorkoutGlobal.VideoService.Api.IntegrationTests
{
    internal class AppTestConnection<TKey>
    {
        private HttpClient _appClient;
        private IConfiguration _configuration;
        private List<TKey> _purgeList;
        private string _baseAddress;

        public AppTestConnection()
        {
            Configuration = ConfigurationAccessor.GetTestConfiguration();
            AppClient = new();
            PurgeList = new();
            BaseAddress = Configuration["BaseHost"];
            AppClient.BaseAddress = new Uri(BaseAddress);
        }

        public string BaseAddress
        {
            get => _baseAddress;
            set => _baseAddress = string.IsNullOrEmpty(value) ? _configuration?["BaseHost"] : value;
        }

        public HttpClient AppClient
        {
            get => _appClient;
            set => _appClient = value;
        }

        public IConfiguration Configuration
        {
            get => _configuration;
            set => _configuration = value ?? ConfigurationAccessor.GetTestConfiguration();
        }

        public List<TKey> PurgeList
        {
            get => _purgeList;
            set => _purgeList = value;
        }
    }
}
