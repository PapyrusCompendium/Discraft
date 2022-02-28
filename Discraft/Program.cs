using System;

using Discord.WebSocket;

using Discraft.Extensions;

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

            host.Run();
        }
    }
}