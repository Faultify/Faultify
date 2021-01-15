using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Faultify.Cli
{
    internal class Program
    {
        private readonly ILogger<Program> _logger;
        private readonly IOptions<Settings> _settings;

        public Program(
            IOptions<Settings> settings,
            ILogger<Program> logger
        )
        {
            _settings = settings;
            _logger = logger;
        }

        private static async Task Main(string[] args)
        {
            var configurationRoot = BuildConfigurationRoot();
            var services = new ServiceCollection();
            services.Configure<Settings>(options => configurationRoot.GetSection("settings").Bind(options));
            services.AddLogging(c => c.AddConsole());
            services.AddSingleton<Program>();
            var serviceProvider = services.BuildServiceProvider();
            var program = serviceProvider.GetService<Program>();
            await program.Run();
        }

        private async Task Run()
        {
        }

        private static IConfigurationRoot BuildConfigurationRoot()
        {
            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets<Program>();
            var configurationRoot = builder.Build();
            return configurationRoot;
        }
    }
}