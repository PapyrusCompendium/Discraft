using System.Diagnostics;

using Discraft.Services.Interfaces;

namespace Discraft.Services {
    public class HostedProcess : Process, IHostedProcess {
        private readonly string _execCommand;

        public HostedProcess(string execCommand) {
            _execCommand = execCommand;

            StartInfo = new ProcessStartInfo {
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };

            OutputDataReceived += HostedProcess_OutputDataReceived;
        }

        private void HostedProcess_OutputDataReceived(object sender, DataReceivedEventArgs e) {

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