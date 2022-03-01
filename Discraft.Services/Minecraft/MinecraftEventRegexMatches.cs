using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Discraft.Services.Minecraft {
    class MinecraftEventRegexMatches {
        /// <summary>
        /// Group 1: Player name
        /// </summary>
        public static Regex JoinedGame = new Regex(@"Server thread\/INFO]: (.+) joined the game");

        /// <summary>
        /// Group 1: Player name
        /// Group 2: IpAddress:Port
        /// Group 3: Mc Entity Id
        /// Group 4: X coordinate
        /// Group 5: Y coordinate
        /// Group 6: Z coordinate
        /// </summary>
        public static Regex SpawnedIn = new Regex(@"Server thread\/INFO]: (.+)\[\/(.+)\] logged in with entity id (\d+) at \((.+), (.+), (.+)\)");

        /// <summary>
        /// Group 1: Player name
        /// </summary>
        public static Regex LeftGame = new Regex(@"Server thread\/INFO\]: (.+) left the game");

        /// <summary>
        /// Group 1: Player name
        /// </summary>
        public static Regex LostConnection = new Regex(@"Server thread\/INFO\]: (.+) lost connection: (.+)");

        /// <summary>
        /// Group 1: Player name
        /// Group 2: User Id
        /// </summary>
        public static Regex Authentication = new Regex(@"User Authenticator #\d\/INFO\]: UUID of player (.+) is (.+)");

        /// <summary>
        /// 
        /// </summary>
        public static Regex DisconnectHandler = new Regex(@"Server thread\/WARN]: handleDisconnection\(\) called twice");

        /// <summary>
        /// Group 1: Player name
        /// Group 2: Message
        /// </summary>
        public static Regex SentMessage = new Regex(@"Server thread\/INFO]: <(.+)> (.+)");

        /// <summary>
        /// Group 1: Joined count
        /// Group 2: Max count
        /// </summary>
        public static Regex PlayerList = new Regex(@"There are (\d+) of a max of (\d+).+");

        /// <summary>
        /// All the Regex matches with their <see cref="ConsoleResponseType"/>
        /// </summary>
        public static Dictionary<MincraftEventTypes, Regex> AllRegexMatches = new Dictionary<MincraftEventTypes, Regex>() {
                {MincraftEventTypes.Authentication, Authentication },
                {MincraftEventTypes.DisconnectHandler, DisconnectHandler },
                {MincraftEventTypes.JoinedGame, JoinedGame },
                {MincraftEventTypes.LeftGame, LeftGame },
                {MincraftEventTypes.LostConnection, LostConnection },
                {MincraftEventTypes.PlayerList, PlayerList },
                {MincraftEventTypes.SentMessage, SentMessage },
                {MincraftEventTypes.SpawnedIn, SpawnedIn },
            };
    }
}
