using System;

using Discraft.Extensions;

using Microsoft.Extensions.Hosting;

namespace Discraft {
    public class Program {
        static void Main(string[] args) {
            var hostBuilder = new HostBuilder()
                .UseConsoleLifetime()
                .UseStartup<DiscraftStartup>()
                .Build();
        }
    }
}