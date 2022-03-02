using System.Text.RegularExpressions;

namespace Discraft.Services.Minecraft.Interfaces {
    public interface ICommandTracker {
        void ClearTrackkedCommands();
        void TrackConsoleOutput(Match logMatch);
    }
}