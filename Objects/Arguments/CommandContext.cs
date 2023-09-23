using izolabella.Discord.Objects.Clients;

namespace izolabella.Discord.Objects.Arguments
{
    /// <summary>
    /// A class containing information used to pass along to the methods of commands.
    /// </summary>
    /// <remarks>
    /// A class containing information used to pass along to the methods of commands.
    /// </remarks>
    public class CommandContext(SocketSlashCommand SlashCommand, IzolabellaDiscordClient Reference)
    {

        /// <summary>
        /// The command context.
        /// </summary>
        public SocketSlashCommand UserContext { get; } = SlashCommand;

        /// <summary>
        /// The handler responsible for dispatching this context.
        /// </summary>
        public IzolabellaDiscordClient Reference { get; } = Reference;
    }
}