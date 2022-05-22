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
        public CommandContext(SocketSlashCommand SlashCommand)
        {
            this.UserContext = SlashCommand;
        }

        /// <summary>
        /// The command context.
        /// </summary>
        public SocketSlashCommand UserContext { get; }
    }
}
