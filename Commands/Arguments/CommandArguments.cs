using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Commands.Arguments
{
    /// <summary>
    /// A class containing information used to pass along to the methods of commands.
    /// </summary>
    public class CommandArguments
    {
        /// <summary>
        /// A class containing information used to pass along to the methods of commands.
        /// </summary>
        public CommandArguments(SocketMessage Message)
        {
            this.Message = Message;
        }

        /// <summary>
        /// The message context for the command.
        /// </summary>
        public SocketMessage Message { get; }
    }
}
