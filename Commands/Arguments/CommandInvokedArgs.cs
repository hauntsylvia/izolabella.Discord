using izolabella.Discord.Internals.Surgical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="Message">The message that invoked the command.</param>
        /// <param name="CommandInvoked">The command that was invoked.</param>
        public CommandInvokedArgs(SocketMessage Message, CommandWrapper CommandInvoked)
        {
            this.Message = Message;
            this.CommandInvoked = CommandInvoked;
        }
        /// <summary>
        /// The message that invoked the command.
        /// </summary>
        public SocketMessage Message { get; }
        /// <summary>
        /// The command that was invoked.
        /// </summary>
        public CommandWrapper CommandInvoked { get; }
    }
}
