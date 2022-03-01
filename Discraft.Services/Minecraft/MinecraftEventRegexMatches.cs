using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Discraft.Services.Minecraft {
    public class MinecraftEventRegexMatches {
        /// <summary>
        /// This regex should capture all valid Logs from default Mincraft.
        /// Group 1: TimeStamp
        /// Group 2: Thread or Worker Id
        /// Group 3: Log Type
        /// Group 4: Log Message
        /// </summary>
        public static Regex MinecraftLog = new(@"\[(.+)\] \[(.+)/(.+)\]: (.+)");

        /// <summary>
        /// Group 1: Player name
        /// </summary>
        public static Regex JoinedGame = new(@"Server thread\/INFO]: (.+) joined the game");

        /// <summary>
        /// Group 1: Player name
        /// Group 2: IpAddress:Port
        /// Group 3: Mc Entity Id
        /// Group 4: X coordinate
        /// Group 5: Y coordinate
        /// Group 6: Z coordinate
        /// </summary>
        public static Regex SpawnedIn = new(@"Server thread\/INFO]: (.+)\[\/(.+)\] logged in with entity id (\d+) at \((.+), (.+), (.+)\)");

        /// <summary>
        /// Group 1: Player name
        /// </summary>
        public static Regex LeftGame = new(@"Server thread\/INFO\]: (.+) left the game");

        /// <summary>
        /// Group 1: Player name
        /// </summary>
        public static Regex LostConnection = new(@"Server thread\/INFO\]: (.+) lost connection: (.+)");

        /// <summary>
        /// Group 1: Player name
        /// Group 2: User Id
        /// </summary>
        public static Regex Authentication = new(@"User Authenticator #\d\/INFO\]: UUID of player (.+) is (.+)");

        /// <summary>
        /// 
        /// </summary>
        public static Regex DisconnectHandler = new(@"Server thread\/WARN]: handleDisconnection\(\) called twice");

        /// <summary>
        /// Group 1: Player name
        /// Group 2: Message
        /// </summary>
        public static Regex SentMessage = new(@"Server thread\/INFO]: <(.+)> (.+)");

        /// <summary>
        /// Group 1: Joined count
        /// Group 2: Max count
        /// </summary>
        public static Regex PlayerList = new(@"There are (\d+) of a max of (\d+).+");

        /// <summary>
        /// All the Regex matches with their <see cref="ConsoleResponseType"/>
        /// </summary>
        public static Dictionary<MincraftEventType, Regex> AllRegexMatches = new Dictionary<MincraftEventType, Regex>() {
                {MincraftEventType.Authentication, Authentication },
                {MincraftEventType.DisconnectHandler, DisconnectHandler },
                {MincraftEventType.JoinedGame, JoinedGame },
                {MincraftEventType.LeftGame, LeftGame },
                {MincraftEventType.LostConnection, LostConnection },
                {MincraftEventType.PlayerList, PlayerList },
                {MincraftEventType.SentMessage, SentMessage },
                {MincraftEventType.SpawnedIn, SpawnedIn },
            };

        public static MincraftEventType CheckRegexEvents(string consoleLine) {
            foreach (var regexObject in AllRegexMatches) {
                var match = regexObject.Value.Match(consoleLine);
                if (match.Success) {
                    return regexObject.Key;
                }
            }

            return MincraftEventType.Unknown;
        }
    }
}
