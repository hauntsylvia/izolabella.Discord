using izolabella.Discord.Internals.Surgical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Commands.Arguments
{
    /// <summary>
    /// A class containing relevant arguments after a command rejection.
    /// </summary>
    public class CommandRejectedArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CommandInvokedArgs"/>
        /// </summary>
        /// <param name="SlashCommand">The message that was rejected.</param>
        /// <param name="CommandRejected">The command that was rejected.</param>
        public CommandRejectedArgs(SocketSlashCommand SlashCommand, CommandWrapper CommandRejected)
        {
            this.SlashCommand = SlashCommand;
            this.CommandRejected = CommandRejected;
        }
        /// <summary>
        /// The command context.
        /// </summary>
        public SocketSlashCommand SlashCommand { get; }

        /// <summary>
        /// The command that was rejected.
        /// </summary>
        public CommandWrapper CommandRejected { get; }
    }
}
