using System;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Discraft.Services.Discord.Interfaces;
using Discraft.Services.Interfaces;

using Microsoft.Extensions.Configuration;

namespace Discraft.Services.Discord {
    public class CommandHandler : ICommandHandler {

        private readonly CommandService _commandService;
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IConfiguration _configuration;

        public CommandHandler(DiscordSocketClient discordSocketClient, CommandService commandService,
            IServiceProvider services, IConfiguration configuration, ILogger logger) {
            _discordSocketClient = discordSocketClient;
            _commandService = commandService;
            _services = services;
            _configuration = configuration;
            _logger = logger;

            _discordSocketClient.MessageReceived += DiscordSocketClientMessageReceived;
            _commandService.CommandExecuted += CommandServiceCommandExecuted;
        }

        public async Task InitializeAsync() {
            await _commandService.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);
            _logger.Debug($"Execute Command: {_configuration["ExecCommand"]}");
            _logger.Debug($"Log File: {_configuration["LogFile"]}");
        }

        private async Task DiscordSocketClientMessageReceived(SocketMessage socketMessage) {
            if (socketMessage is not IUserMessage userMessage) {
                return;
            }

            var argumentPosistion = 0;
            if (!userMessage.HasMentionPrefix(_discordSocketClient.CurrentUser, ref argumentPosistion)) {
                return;
            }

            var context = new CommandContext(_discordSocketClient, userMessage);

            _logger.Debug($"Got Command: {socketMessage.Content.Substring(argumentPosistion)}");
            await _commandService.ExecuteAsync(context, argumentPosistion, _services);
        }

        private async Task CommandServiceCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext context, IResult commandResult) {
            if (!commandInfo.IsSpecified || commandResult.IsSuccess) {
                return;
            }

            _logger.Error($"[{context.User.Username}::{commandInfo.Value.Name}] error: {commandResult}");
            await context.Channel.SendMessageAsync($"[{context.User.Username}::{commandInfo.Value.Name}] error: {commandResult}");
        }
    }
}