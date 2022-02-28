using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Discraft.Interfaces {
    public interface IStartup {

        IConfiguration Configuration { get; set; }

        void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services);

        void Configure(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder);
    }
}