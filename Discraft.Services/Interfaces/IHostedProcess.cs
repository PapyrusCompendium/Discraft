namespace Discraft.Services.Interfaces {
    public interface IHostedProcess {
        void RestartProcess();
        void StartProcess();
        void StopProcess();
    }
}