using System;

using Discraft.Extensions;
using Discraft.Services.Discord;
using Discraft.Services.Discord.Interfaces;

using Microsoft.Extensions.Hosting;

namespace Discraft {
    public class Program {
        static void Main(string[] args) {
            if (!(OperatingSystem.IsLinux() || OperatingSystem.IsWindows() || OperatingSystem.IsMacOS())) {
                throw new PlatformNotSupportedException("Only Linux, Windows, and Mac are supported.");
            }

            var host = new HostBuilder()
                .UseConsoleLifetime()
                .UseStartup<DiscraftStartup>()
                .Build();

            var commandHandler = (CommandHandler)host.Services.GetService(typeof(ICommandHandler));
            commandHandler.InitializeAsync().Wait();

            host.Run();
        }
    }
}