using izolabella.Discord.Internals.Surgical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Commands.Arguments
{
    public class CommandRejectedArgs
    {
        public CommandRejectedArgs(SocketMessage Message, CommandWrapper CommandRejected)
        {
            this.Message = Message;
            this.CommandRejected = CommandRejected;
        }

        public SocketMessage Message { get; }
        public CommandWrapper CommandRejected { get; }
    }
}
