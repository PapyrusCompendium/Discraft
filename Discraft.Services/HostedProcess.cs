using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

using Discraft.Services.Interfaces;
using Discraft.Services.Minecraft;

using Microsoft.Extensions.Configuration;

namespace Discraft.Services {
    public class HostedProcess : IHostedProcess {
        private const string PROCESS_ID_PATH = "server.process";

        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly TimeSpan _restartDelay = TimeSpan.FromSeconds(5);
        private readonly int _maxRetries = 5;

        private Process _serverProcess;
        private bool _shouldRestart = false;
        private DateTime _lastRetry = DateTime.Now;
        private int _retries = 0;

        private Dictionary<MincraftEventType, Stack<Match>> _commandResponses = new();

        ~HostedProcess() {
            _serverProcess.Kill(true);
            _serverProcess.Dispose();
            _serverProcess = null;

            File.Delete(PROCESS_ID_PATH);
        }

        public HostedProcess(IConfiguration configuration, ILogger logger) {
            _configuration = configuration;
            _logger = logger;

            KillStaleProcess();

            _logger.Info($"Executing program: {_configuration["ExecProgram"]}");
            _logger.Info($"Program arguments: {_configuration["ExecArguments"]}");
            _logger.Info($"Working directory: {_configuration["WorkingDirectory"]}");
        }

        private void ConstructStartInfo() {
            _serverProcess = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = _configuration["ExecProgram"],
                    Arguments = _configuration["ExecArguments"],
                    WorkingDirectory = _configuration["WorkingDirectory"],
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true
                },

                EnableRaisingEvents = true
            };

            _serverProcess.Exited += HostedProcess_Exited;
            _serverProcess.OutputDataReceived += HostedProcess_OutputDataReceived;
            _serverProcess.ErrorDataReceived += HostedProcess_ErrorDataReceived;
        }

        private void KillStaleProcess() {
            if (File.Exists(PROCESS_ID_PATH)) {
                try {
                    if (int.TryParse(File.ReadAllText(PROCESS_ID_PATH), out var existingID)) {
                        var existingProcess = Process.GetProcessById(existingID);
                        if (existingProcess.ProcessName == _configuration["ExecProgram"]) {
                            _logger.Warning($"Killing stale server process {existingProcess.Id}");
                            existingProcess.Kill(true);
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
            var consoleMessage = dataReceivedEventArgs?.Data ?? string.Empty;
            var logMatch = MinecraftEventRegexMatches.MinecraftLog.Match(consoleMessage);
            if (!logMatch.Success) {
                return;
            }

            _logger.Info($"[Minecraft] {logMatch.Groups[4]}");

            var eventType = MinecraftEventRegexMatches.CheckRegexEvents(consoleMessage);
            if (eventType == MincraftEventType.Unknown) {
                return;
            }

            if (!_commandResponses.ContainsKey(eventType)) {
                _commandResponses.Add(eventType, new Stack<Match>());
            }

            var match = MinecraftEventRegexMatches.AllRegexMatches[eventType].Match(consoleMessage);
            _commandResponses[eventType].Push(match);
        }

        public void RestartProcess() {
            StopProcess();
            StartProcess();
        }

        public bool StopProcess() {
            if (_serverProcess is null) {
                _logger.Info("Server already stopped.");
                return false;
            }

            _shouldRestart = false;

            _logger.Warning($"Stopping Mincraft, Process ID: {_serverProcess.Id}");

            _serverProcess.Kill(true);
            _serverProcess.WaitForExit((int)TimeSpan.FromSeconds(10).TotalMilliseconds);
            _serverProcess.Dispose();
            _serverProcess = null;

            return true;
        }

        public bool StartProcess() {
            if (_serverProcess?.Responding ?? false) {
                _logger.Info("Server already running.");
                return false;
            }

            ConstructStartInfo();

            _shouldRestart = true;
            _serverProcess.Start();

            try {
                File.Delete(PROCESS_ID_PATH);
                File.WriteAllText(PROCESS_ID_PATH, _serverProcess.Id.ToString());
            }
            catch (IOException ioException) {
                _logger.Error($"Could not update file server.process: {ioException.Message}");
            }

            try {
                _serverProcess.BeginOutputReadLine();
                _serverProcess.BeginErrorReadLine();
            }
            catch (InvalidOperationException) {

            }

            _logger.Warning($"Starting Mincraft Process ID: {_serverProcess.Id}");
            return true;
        }

        public void SendStdIn(string commandInput) {
            _serverProcess.StandardInput.WriteLine(commandInput);
        }

        public Match GetCommandResponse(string commandInput, MincraftEventType excpectedResponseType) {
            SendStdIn(commandInput);

            // Stack count is O(1) time complexity
            var currentStackSize = _commandResponses.ContainsKey(excpectedResponseType)
                ? _commandResponses[excpectedResponseType].Count
                : 0;

            var waitUntilDate = DateTime.Now.AddSeconds(5);

            while (!_commandResponses.ContainsKey(excpectedResponseType)
                || _commandResponses[excpectedResponseType].Count < currentStackSize
                || DateTime.Now > waitUntilDate) {
                Thread.Sleep(500);
            }

            return _commandResponses[excpectedResponseType].Count == currentStackSize
                ? Match.Empty
                : _commandResponses[excpectedResponseType].Pop();
        }
    }
}