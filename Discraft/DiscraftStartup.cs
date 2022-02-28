
using Discraft.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Discraft {
    public class DiscraftStartup : IStartup {
        public IConfiguration Configuration { get; set; }

        public void Configure(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder) {

        }

        public void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services) {

        }
    }
}