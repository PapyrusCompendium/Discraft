using System;

using Discraft.Extensions;
using Discraft.Services;
using Discraft.Services.Discord;
using Discraft.Services.Discord.Interfaces;
using Discraft.Services.Interfaces;

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

            // Init our discord bot
            var commandHandler = (CommandHandler)host.Services.GetService(typeof(ICommandHandler));
            commandHandler.InitializeAsync().Wait();

            // Init our hosted Mincraft process.
            var minecraft = (HostedProcess)host.Services.GetService(typeof(IHostedProcess));
            minecraft.StartProcess();

            host.Run();
        }
    }
}