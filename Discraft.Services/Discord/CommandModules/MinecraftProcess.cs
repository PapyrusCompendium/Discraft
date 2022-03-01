using System.Threading.Tasks;

using Discord.Commands;

using Discraft.Services.Interfaces;


namespace Discraft.Services.Discord.CommandModules {
    public class MinecraftProcess : ModuleBase<CommandContext> {
        private readonly IHostedProcess _hostedProcess;
        private readonly ILogger _logger;

        public MinecraftProcess(IHostedProcess hostedProcess, ILogger logger) {
            _hostedProcess = hostedProcess;
            _logger = logger;
        }

        [Command("Restart"), Summary("Restarts the minecraft server.")]
        public async Task RestartAsync() {
            _logger.Info("Restarting server from command.");
            _hostedProcess.RestartProcess();

            await ReplyAsync("Restarted Server.");
        }

        [Command("Start"), Summary("Start the minecraft server.")]
        public async Task StartAsync() {
            _logger.Info("Starting server from command.");
            var didStart = _hostedProcess.StartProcess();

            var response = didStart
                ? "Successfully started server."
                : "Server was already running.";

            await ReplyAsync(response);
        }

        [Command("Stop"), Summary("Stops the minecraft server.")]
        public async Task StopAsync() {
            _logger.Info("Stopping server from command.");
            var didStop = _hostedProcess.StopProcess();

            var response = didStop
                ? "Successfully stopped server."
                : "Server was not running.";

            await ReplyAsync(response);
        }
    }
}
