using Discraft.Interfaces;
using Discraft.Services;
using Discraft.Services.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Discraft {
    public class DiscraftStartup : IStartup {
        public IConfiguration Configuration { get; set; }

        public void Configure(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder) {
            configurationBuilder.AddEnvironmentVariables()
                .AddJsonFile("AppSettings.json", false)
                .AddJsonFile("AppSettings.Development.json", true);
        }

        public void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services) {
            services.AddSingleton<IHostedProcess>(new HostedProcess(Configuration["ExecCommand"]));
        }
    }
}