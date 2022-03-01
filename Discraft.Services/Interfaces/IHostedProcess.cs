namespace Discraft.Services.Interfaces {
    public interface IHostedProcess {
        void RestartProcess();
        void SendStdIn(string commandInput);
        bool StartProcess();
        bool StopProcess();
    }
}