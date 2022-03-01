using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Discraft.Interfaces;
using Discraft.Services;
using Discraft.Services.Discord;
using Discraft.Services.Discord.Interfaces;
using Discraft.Services.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Discraft {
    public class DiscraftStartup : IStartup {
        public IConfiguration Configuration { get; set; }

        public void Configure(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder) {
            configurationBuilder
                .AddJsonFile("AppSettings.json", false)
                .AddJsonFile("AppSettings.Development.json", true)
                .AddEnvironmentVariables();
        }

        public void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services) {
            var socketClient = new DiscordSocketClient(new DiscordSocketConfig {
                LogGatewayIntentWarnings = true,
                LogLevel = LogSeverity.Verbose
            });

            socketClient.LoginAsync(TokenType.Bot, Configuration["DiscordBotToken"]).Wait();
            socketClient.StartAsync().Wait();

            services
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IHostedProcess>(new HostedProcess(Configuration["ExecCommand"]))
                .AddSingleton(socketClient)
                .AddSingleton<ICommandHandler, CommandHandler>()
                .AddSingleton(new CommandService());
        }
    }
}