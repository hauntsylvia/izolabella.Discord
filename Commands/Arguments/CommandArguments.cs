using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Commands.Arguments
{
    public class CommandArguments
    {
        public CommandArguments(SocketMessage Message)
        {
            this.Message = Message;
        }

        public SocketMessage Message { get; }
    }
}
