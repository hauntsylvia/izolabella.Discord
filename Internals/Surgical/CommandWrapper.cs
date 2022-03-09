using izolabella.Discord.Commands.Arguments;
using izolabella.Discord.Commands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Internals.Surgical
{
    public class CommandWrapper
    {
        public CommandWrapper(CommandAttribute Attribute, MethodInfo MethodInfo)
        {
            this.Attribute = Attribute;
            this.MethodInfo = MethodInfo;
        }

        public CommandAttribute Attribute { get; }
        private MethodInfo MethodInfo { get; }

        public void InvokeThis(SocketMessage Context)
        {
            this.MethodInfo.Invoke(this.Attribute, new object[] { new CommandArguments(Context) });
        }
    }
}
