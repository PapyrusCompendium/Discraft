using System.Text.RegularExpressions;

using Discraft.Services.Minecraft;

namespace Discraft.Services.Interfaces {
    public interface IHostedProcess {
        Match GetCommandResponse(string commandInput, MincraftEventType excpectedResponseType);
        void RestartProcess();
        void SendStdIn(string commandInput);
        bool StartProcess();
        bool StopProcess();
    }
}