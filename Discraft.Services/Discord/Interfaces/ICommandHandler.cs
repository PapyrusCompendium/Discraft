using Discord.WebSocket;

using Microsoft.Extensions.Configuration;

namespace Discraft.Services.Discord.Interfaces {
    public interface ICommandHandler {
        IConfiguration Configuration { get; }
        DiscordSocketClient DiscordSocketClient { get; }
    }
}