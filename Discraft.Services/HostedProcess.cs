using System.Diagnostics;

using Discraft.Services.Interfaces;

namespace Discraft.Services {
    public class HostedProcess : Process, IHostedProcess {
        private readonly string _execCommand;

        public HostedProcess(string execCommand) {
            _execCommand = execCommand;
        }

        public void RestartProcess() {
            StopProcess();
            StartProcess();
        }

        public void StopProcess() {

        }

        public void StartProcess() {

        }
    }
}