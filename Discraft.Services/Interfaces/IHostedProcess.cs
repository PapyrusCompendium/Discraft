namespace Discraft.Services.Interfaces {
    public interface IHostedProcess {
        void RestartProcess();
        bool StartProcess();
        bool StopProcess();
    }
}