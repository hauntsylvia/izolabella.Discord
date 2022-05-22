using izolabella.Discord.Objects.Arguments;
using izolabella.Discord.Objects.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Objects.Interfaces
{
    /// <summary>
    /// An interface that defines commands by classes.
    /// </summary>
    public interface IIzolabellaCommand
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The description of the command.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The expected parameters.
        /// </summary>
        public IzolabellaCommandParameter[] Parameters { get; }

        /// <summary>
        /// The method that will run when the command is invoked.
        /// </summary>
        /// <param name="Context">The context the handler will pass.</param>
        /// <param name="Arguments">The arguments the end user has invoked this command with.</param>
        /// <returns></returns>
        public Task RunAsync(CommandContext Context, IzolabellaCommandArgument[] Arguments);
    }
}
