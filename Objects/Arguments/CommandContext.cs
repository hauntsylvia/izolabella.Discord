using izolabella.Discord.Objects.Clients;

namespace izolabella.Discord.Objects.Arguments
{
    /// <summary>
    /// A class containing information used to pass along to the methods of commands.
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// A class containing information used to pass along to the methods of commands.
        /// </summary>
        public CommandContext(SocketSlashCommand SlashCommand, IzolabellaDiscordClient Reference)
        {
            this.UserContext = SlashCommand;
            this.Reference = Reference;
        }

        /// <summary>
        /// The command context.
        /// </summary>
        public SocketSlashCommand UserContext { get; }

        /// <summary>
        /// The handler responsible for dispatching this context.
        /// </summary>
        public IzolabellaDiscordClient Reference { get; }
    }
}