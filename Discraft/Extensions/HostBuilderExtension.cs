using Discraft.Interfaces;

using Microsoft.Extensions.Hosting;

namespace Discraft.Extensions {
    public static class HostBuilderExtension {
        public static IHostBuilder UseStartup<T>(this IHostBuilder hostBuilder) where T : IStartup, new() {
            IStartup startup = new T();

            hostBuilder.ConfigureAppConfiguration(startup.Configure);

            hostBuilder.ConfigureAppConfiguration((_, config) => startup.Configuration = config.Build());
            hostBuilder.ConfigureServices(startup.ConfigureServices);

            return hostBuilder;
        }
    }
}