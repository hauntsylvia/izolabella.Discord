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
        public CommandArguments(SocketSlashCommand SlashCommand)
        {
            this.SlashCommand = SlashCommand;
        }

        /// <summary>
        /// The command context.
        /// </summary>
        public SocketSlashCommand SlashCommand { get; }
    }
}
