using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Discraft.Services.Interfaces;
using Discraft.Services.Minecraft;

using Microsoft.Extensions.Configuration;

namespace Discraft.Services {
    public class HostedProcess : Process, IHostedProcess {
        private const string PROCESS_ID_PATH = "server.process";

        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly TimeSpan _restartDelay = TimeSpan.FromSeconds(5);
        private readonly int _maxRetries = 5;

        private bool _shouldRestart = false;
        private DateTime _lastRetry = DateTime.Now;
        private int _retries = 0;

        ~HostedProcess() {
            Kill();
            File.Delete(PROCESS_ID_PATH);
        }

        public HostedProcess(IConfiguration configuration, ILogger logger) {
            _configuration = configuration;
            _logger = logger;

            KillStaleProcess();

            _logger.Info($"Executing program: {_configuration["ExecProgram"]}");
            _logger.Info($"Program arguments: {_configuration["ExecArguments"]}");
            _logger.Info($"Working directory: {_configuration["WorkingDirectory"]}");

            StartInfo = new ProcessStartInfo {
                FileName = _configuration["ExecProgram"],
                Arguments = _configuration["ExecArguments"],
                WorkingDirectory = _configuration["WorkingDirectory"],
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true
            };

            EnableRaisingEvents = true;
            Exited += HostedProcess_Exited;
            OutputDataReceived += HostedProcess_OutputDataReceived;
            ErrorDataReceived += HostedProcess_ErrorDataReceived;
        }

        private void KillStaleProcess() {
            if (File.Exists(PROCESS_ID_PATH)) {
                try {
                    if (int.TryParse(File.ReadAllText(PROCESS_ID_PATH), out var existingID)) {
                        var existingProcess = GetProcessById(existingID);
                        if (existingProcess.ProcessName == _configuration["ExecProgram"]) {
                            _logger.Warning($"Killing stale server process {existingProcess.Id}");
                            existingProcess.Kill();
                        }
                    }
                }
                catch (IOException ioException) {
                    _logger.Error($"Could not read server.process: {ioException.Message}");
                }
                catch (ArgumentException) {
                    _logger.Debug("Cleaning old process id, was not running.");
                    File.Delete(PROCESS_ID_PATH);
                }
            }
        }

        private void HostedProcess_Exited(object sender, EventArgs e) {
            if (!_shouldRestart) {
                _logger.Info("Server Shutdown.");
                return;
            }

            if (DateTime.Now.Subtract(_lastRetry).TotalMinutes >= 2) {
                _retries = 0;
            }

            if (_retries >= _maxRetries) {
                _logger.Error($"Current retries exceeds max retries of {_maxRetries}. Stopping service.");
                return;
            }

            _logger.Error($"Crash detected, restarting server after {_restartDelay.TotalSeconds} seconds...");
            Thread.Sleep(_restartDelay);
            RestartProcess();

            _lastRetry = DateTime.Now;
            _retries++;
        }

        private void HostedProcess_ErrorDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs) {
            _logger.Error(dataReceivedEventArgs.Data);
        }

        private void HostedProcess_OutputDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs) {
            var logMatch = MinecraftEventRegexMatches.MinecraftLog.Match(dataReceivedEventArgs?.Data ?? string.Empty);
            if (!logMatch.Success) {
                return;
            }

            _logger.Info($"[Minecraft] {logMatch.Groups[4]}");
        }

        public void RestartProcess() {
            StopProcess();
            StartProcess();
        }

        public void StopProcess() {
            _shouldRestart = false;

            CancelOutputRead();
            CancelErrorRead();
            Kill();
            _logger.Warning($"Stopping Mincraft, Process ID: {Id}");
        }

        public void StartProcess() {
            _shouldRestart = true;
            Start();

            try {
                File.Delete(PROCESS_ID_PATH);
                File.WriteAllText(PROCESS_ID_PATH, Id.ToString());
            }
            catch (IOException ioException) {
                _logger.Error($"Could not update file server.process: {ioException.Message}");
            }

            try {
                BeginOutputReadLine();
                BeginErrorReadLine();
            }
            catch (InvalidOperationException) {
                CancelOutputRead();
                CancelErrorRead();

                BeginOutputReadLine();
                BeginErrorReadLine();
            }

            _logger.Warning($"Starting Mincraft Process ID: {Id}");
        }
    }
}