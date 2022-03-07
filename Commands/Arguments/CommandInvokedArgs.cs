using izolabella.Discord.Internals.Surgical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Commands.Arguments
{
    public class CommandInvokedArgs
    {
        public CommandInvokedArgs(SocketMessage Message, CommandWrapper CommandInvoked)
        {
            this.Message = Message;
            this.CommandRejected = CommandInvoked;
        }

        public SocketMessage Message { get; }
        public CommandWrapper CommandRejected { get; }
    }
}
