namespace Discraft.Services.Minecraft {
    public enum MincraftEventType {
        JoinedGame,
        Authentication,
        SpawnedIn,
        LeftGame,
        LostConnection,
        DisconnectHandler,
        SentMessage,
        /// <summary>
        /// Group 1: Joined
        /// Group 2: Max
        /// </summary>
        PlayerList,
        Unknown
    }
}