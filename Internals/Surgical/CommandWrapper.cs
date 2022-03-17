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
    /// <summary>
    /// A class containing information for ease of information passing.
    /// </summary>
    public class CommandWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandWrapper"/> class.
        /// </summary>
        /// <param name="Attribute">The attribute that applies to this command.</param>
        /// <param name="MethodInfo">The method to invoke for this command.</param>
        public CommandWrapper(CommandAttribute Attribute, MethodInfo MethodInfo)
        {
            this.Attribute = Attribute;
            this.MethodInfo = MethodInfo;
        }

        /// <summary>
        /// This command's attribute.
        /// </summary>
        public CommandAttribute Attribute { get; }
        /// <summary>
        /// The slash command compliant name for this command.
        /// </summary>
        public string SlashCommandTag => this.Attribute.Tags.First().ToLower().Replace(' ', '-');
        private MethodInfo MethodInfo { get; }

        /// <summary>
        /// Invoke the wrapped <see cref="MethodInfo"/> for this command.
        /// </summary>
        /// <param name="Context">The message that invoked this command.</param>
        public void InvokeThis(SocketSlashCommand Context)
        {
            this.MethodInfo.Invoke(this.Attribute, new object[] { new CommandArguments(Context) });
        }
    }
}
