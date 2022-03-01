using System.Threading.Tasks;

using Discord.Commands;

using Discraft.Services.Interfaces;

namespace Discraft.Services.Discord.CommandModules {
    public class MinecraftCommands : ModuleBase<CommandContext> {
        private readonly IHostedProcess _hostedProcess;
        private readonly ILogger _logger;

        public MinecraftCommands(IHostedProcess hostedProcess, ILogger logger) {
            _hostedProcess = hostedProcess;
            _logger = logger;
        }

        [Command("Say"), Summary("Prints a chat to the server.")]
        public async Task RestartAsync([Remainder] string message) {
            _logger.Info($"Sending '{message}' chat message.");
            _hostedProcess.SendStdIn($"/say {message}");

            await ReplyAsync("Command Executed.");
        }

        [Command("Players"), Summary("Shows how many players are online.")]
        public async Task PlayerCountAsync() {
            var match = _hostedProcess.GetCommandResponse("/list", Minecraft.MincraftEventType.PlayerList);

            await ReplyAsync($"There are {match.Groups[1]} out of {match.Groups[2]} players online.");
        }
    }
}