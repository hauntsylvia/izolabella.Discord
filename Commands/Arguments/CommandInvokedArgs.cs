using izolabella.Discord.Internals.Surgical;

namespace izolabella.Discord.Commands.Arguments
{
    /// <summary>
    /// A class containing relevant arguments after a command invokation.
    /// </summary>
    public class CommandInvokedArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CommandInvokedArgs"/>
        /// </summary>
        /// <param name="SlashCommand">The command context.</param>
        /// <param name="CommandInvoked">The command that was invoked.</param>
        public CommandInvokedArgs(SocketSlashCommand SlashCommand, CommandWrapper CommandInvoked)
        {
            this.SlashCommand = SlashCommand;
            this.CommandInvoked = CommandInvoked;
        }
        /// <summary>
        /// The command context.
        /// </summary>
        public SocketSlashCommand SlashCommand { get; }
        /// <summary>
        /// The command that was invoked.
        /// </summary>
        public CommandWrapper CommandInvoked { get; }
    }
}
