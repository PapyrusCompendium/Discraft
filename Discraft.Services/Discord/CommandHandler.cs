using System.Threading.Tasks;

using Discord.WebSocket;

using Discraft.Services.Discord.Interfaces;
using Discraft.Services.Interfaces;

using Microsoft.Extensions.Configuration;

namespace Discraft.Services.Discord {
    public class CommandHandler : ICommandHandler {
        private readonly ILogger _logger;

        public DiscordSocketClient DiscordSocketClient { get; }
        public IConfiguration Configuration { get; }

        public CommandHandler(DiscordSocketClient discordSocketClient, IConfiguration configuration, ILogger logger) {
            DiscordSocketClient = discordSocketClient;
            Configuration = configuration;
            _logger = logger;
            DiscordSocketClient.MessageReceived += DiscordSocketClient_MessageReceived;

            _logger.Debug($"Command Prefix: {Configuration["CommandPrefix"]}");
        }

        private async Task DiscordSocketClient_MessageReceived(SocketMessage socketMessage) {
            if (socketMessage.Author.IsBot || !socketMessage.Content.StartsWith(Configuration["CommandPrefix"])) {
                return;
            }

            _logger.Debug($"Got Command: {socketMessage.Content}");
        }
    }
}