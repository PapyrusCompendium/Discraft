using System;

using Discraft.Extensions;

using Microsoft.Extensions.Hosting;

namespace Discraft {
    public class Program {
        static void Main(string[] args) {
            if (!(OperatingSystem.IsLinux() || OperatingSystem.IsWindows() || OperatingSystem.IsMacOS())) {
                throw new PlatformNotSupportedException("Only Linux, Windows, and Mac are supported.");
            }

            var hostBuilder = new HostBuilder()
                .UseConsoleLifetime()
                .UseStartup<DiscraftStartup>();

            hostBuilder
                .Build()
                .Start();
        }
    }
}