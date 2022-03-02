using System.Text.RegularExpressions;

using Discraft.Services.Minecraft;

namespace Discraft.Services.Minecraft.Interfaces {
    public interface IMinecraftServer {
        Match GetCommandResponse(string commandInput, MincraftEventType excpectedResponseType);
        void RestartProcess();
        void SendStdIn(string commandInput);
        bool StartProcess();
        bool StopProcess();
    }
}