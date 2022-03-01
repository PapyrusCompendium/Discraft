using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

using Discraft.Services.Interfaces;

using Microsoft.Extensions.Configuration;

namespace Discraft.Services.Discord.CommandModules {
    public class Informational : ModuleBase<CommandContext> {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ILogger _logger;
        private readonly IConfiguration _configurationBuilder;

        public Informational(DiscordSocketClient discordSocketClient, ILogger logger, IConfiguration configurationBuilder) {
            _discordSocketClient = discordSocketClient;
            _logger = logger;
            _configurationBuilder = configurationBuilder;
        }

        [Command("info"), Summary("This command provides information to the user.")]
        public async Task InfoAsync() {
            var message = $@"Hi {Context.User}!
Logs are being written at: {_configurationBuilder["LogFile"]}";
            await ReplyAsync(message);
        }

        [Command("Awoo"), Summary("Foxies go Awooooooo!")]
        public async Task AwooAsync() {
            await ReplyAsync("Awooooooooooo!");
        }
    }
}