
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Discraft.Services.Interfaces;
using Discraft.Services.Minecraft.Interfaces;

namespace Discraft.Services.Minecraft {
    public class CommandTracker : ICommandTracker {
        private readonly Dictionary<MincraftEventType, Stack<Match>> _commandResponses = new();

        private readonly ILogger _logger;

        public CommandTracker(ILogger logger) {
            _logger = logger;
        }

        public void TrackConsoleOutput(Match logMatch) {
            var eventType = MinecraftEventRegexMatches.CheckRegexEvents(logMatch.Value, out var matchedCommandResponse);
            if (!MinecraftEventRegexMatches.CommandResponses.Contains(eventType)) {
                return;
            }

            if (!_commandResponses.ContainsKey(eventType)) {
                _commandResponses.Add(eventType, new Stack<Match>());
                _logger.Debug("Instantiating a new stack.");
            }

            _commandResponses[eventType].Push(matchedCommandResponse);
            _logger.Debug($"Pushed new event {eventType}, total {_commandResponses[eventType].Count} stack items.");
        }

        public void ClearTrackkedCommands() {
            _commandResponses.Clear();
        }
    }
}