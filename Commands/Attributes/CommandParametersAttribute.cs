using izolabella.Discord.Internals.Structures.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Discord.Commands.Attributes
{
    /// <summary>
    /// A class to indicate to the command handler that the attached slash command should have parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandParametersAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandParametersAttribute"/> class.
        /// </summary>
        /// <param name="Parameters">A list of all parameters for the relevant slash command</param>
        public CommandParametersAttribute(IReadOnlyCollection<CommandParameter> Parameters)
        {
            this.Parameters = Parameters;
        }

        /// <summary>
        /// A list of all the parameters.
        /// </summary>
        public IReadOnlyCollection<CommandParameter> Parameters { get; }
    }
}
